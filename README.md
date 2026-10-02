# P2Checker — Phase 2 Translator Board Self-Test

NI STS C# test program for verifying GP3 translator board health prior to DUT testing.

## Project Structure

```
├── Code Modules/
│   ├── InstrCtrl/          — Instrument control wrappers (DCPower, Digital, DMM, DAQmx, etc.)
│   ├── MFlexMigration/     — MFlex-to-GP3 migration framework (HMOD, relay, pattern exec)
│   ├── TestSteps/
│   │   ├── Common/         — Shared helpers (MeterInstance, DAQmxRelay)
│   │   ├── P2Checker/      — Phase 2 board check test steps
│   │   └── P1Checker/      — Phase 1 checkerboard test steps (migrated)
│   └── TBChecker.sln       — Visual Studio solution
├── Limits/                  — Test limit files
├── STDF File/               — STDF result files
├── Supporting Materials/
│   ├── Digital/             — Compiled patterns, waveforms, levels, timing
│   ├── Pin Maps/            — TBChecker.pinmap
│   ├── Calibration Data/
│   └── Offline Configuration/
├── bin/                     — Pre-built DLLs and NI driver assemblies
└── STSCsharp_TBChecker.seq  — TestStand sequence file
```

## Test Modules

### P2Checker (Phase 2)

| Module | Description |
|--------|-------------|
| DIB_Supply_Check | Verifies on-board power supply voltages via DMM |
| DC90_SL23_Check | DC90V channel connectivity check |
| DC30_SL04_Check | DC30V Slot 4 open/short check |
| DC30_SL10_Check | DC30V Slot 10 open/short check |
| DC30_SL24_Check | DC30V Slot 24 open/short check |
| SL04_DC30_DA_Check | DC30 Slot 4 DIB Access check |
| SL10_DC30_DA_Check | DC30 Slot 10 DIB Access check |
| SL24_DC30_DA_Check | DC30 Slot 24 DIB Access check |
| SL04_DC30_DGS_Check | DC30 Slot 4 DGS relay check |
| SL10_DC30_DGS_Check | DC30 Slot 10 DGS relay check |
| SL24_DC30_DGS_Check | DC30 Slot 24 DGS relay check |
| DMM_SL14_Check | PXIe-4081 DMM validation |
| SPI_Pins_Check | SPI pin connectivity check |
| SL12_POOL_Check | POOL (PL OUT) RC rise time, slot 12, 8 channels (`SL12PoolRiseTimeCheck`) |
| SL14_POOL_Check | POOL (PL OUT) RC rise time, slot 14, 8 channels (`SL14PoolRiseTimeCheck`) |
| SL21_POOL_Check | POOL (PL OUT) RC rise time, slot 21, 8 channels (`SL21PoolRiseTimeCheck`) |
| POOL_Input_Probe | Bench diagnostic, reports the DC level at the TFE input for any slot/channel/input option (`PoolInputProbe`) |
| SL04_DIFFMETER_Check | SL04 differential meter check via HMOD24 + HMOD13 K12/K15 |
| SL10_DIFFMETER_Check | SL10 differential meter check via HMOD24 (chPair 1–5) / HMOD25 (chPair 6–10) + HMOD13 K13/K16 |
| SL24_DIFFMETER_Check | SL24 differential meter check via HMOD25 + HMOD13 K14/K17 |

#### POOL relay map

`PoolPathMap` in `P2Checker/POOL_Input_Probe.cs` derives every translator board relay in the
3 slot × 8 channel × 3 input option matrix from one rule: each channel owns nine consecutive
relays, packed across HMOD11, HMOD12 and HMOD13 with no gaps, starting at global relay
`1 + 9 × (channel − 1)` where HMOD11 K1 is 1 and HMOD13 K32 is 96. Offsets within a block are
slot select 0/1/2 for SL12/SL14/SL21, input option 3/4/5 for 50 Ω 3.3 V, 50 Ω 10 V and
10 kΩ 10 V, then changeover 6, scope tap 7 and digital tap 8.

The map reproduces all five literal tables in `SL12_POOL_Check` exactly, including the channel
4 and channel 8 straddles where a block crosses a register boundary. Checker board CHMOD6
drive relays are K16/K18/K20 per slot; channel 1 fan-out interleaves at K17/K19/K21 and
channels 2–8 occupy a contiguous K22–K42 block, three per channel in slot order.

#### POOL input option probe

`POOL_Input_Probe.PoolInputProbe` is a one-off bench step, not a production test: it forces a
DC level on the slot's HSD200 drive pin, settles the RC, and PPMU-reads the channel's 6571 DIO
pin. It sweeps the drive level and publishes each reading so the **slope** identifies which
node the DIO pin actually reaches — 1.0 for the drive side of the 1 kΩ, 0.909 for the RC node
before the 15K divider, 0.4545 for `TFE_IN` after it, 0.0476 for either 50 Ω option, and ~0
for no DC path at all. A near-zero slope parked near −1.0 V or −1.7 V means the changeover does
not bypass the comparator, so the full scale must keep coming from the VOH bisection.

HMOD11 K7 and its per-channel equivalents are an **input**-path changeover, not an output one.
De-energised the divided node feeds the AD96687BRZ window comparator, which is normal POOL
operation; energised it is diverted to the 6571 DIO pin, bypassing the comparator and the XOR.
So the rise time steps must leave it open and the probe closes it — `divertToDio` defaults to
true. Note the `OutputChangeover` comment in the `SL*_POOL_Check` files describes K7 as
selecting between the 6571 and the 5172, which is wrong; the measurement path is unaffected
because the digital strategy leaves the relay open regardless.

The 50 Ω 3.3 V option is rated at the relay input rather than the attenuated node, so the drive
sweep is clamped to 3.3 V for that option only.

#### Measured input option model

Probe results, with the DIO pin diverted to the node by K7:

| Option | R_in | Post-tap gain | Drive | Predicted | Measured | Residual |
|---|---|---|---|---|---|---|
| 50 Ω 3.3 V | 50 Ω | 1.0 | 3.3 V | 157.1 mV | 156 mV | −1.1 mV |
| 50 Ω 10 V | 50 Ω | 0.5 | 5 V | 119.0 mV | 118 mV | −1.0 mV |
| 10 kΩ 10 V | 10 kΩ | 0.5 | 5 V | 2272.7 mV | 2270 mV | −2.7 mV |

The 10 kΩ row confirms `ComparatorFullScaleVolts = 2.2727` and `TauLoadedSec = 909.09 µs`
directly, so the loaded RC model is settled. The residuals are a roughly constant −1 mV rather
than a constant percentage, so they are PPMU offset and not gain error — the resistor ratios are
good to a few tenths of a percent, which rules out drive droop as a cause of a long 10/90 width.

The two 50 Ω options have **different** gains: the 3.3 V option taps the node directly, the 10 V
option divides by two through R62/R63. This is not legible from the schematic text.

Derived per option, with R_eff = 1k ∥ R_in:

| Option | τ | 10/90 | 20/80 | 30/70 |
|---|---|---|---|---|
| 50 Ω (both) | 47.62 µs | 104.6 µs | 66.0 µs | 40.3 µs |
| 10 kΩ | 909.09 µs | 1997.5 µs | 1260.3 µs | 770.3 µs |

The width ratios are 1.5850 and 1.6361 for *every* option, since they cancel τ and amplitude
alike, so one pair of ratio limits covers all nine slot × option combinations.

#### Calibration sequence

`POOL_Calibration.PoolFullScaleCalibration` runs in a **Calibration** sequence ahead of
MainSequence and measures the comparator full scale for every slot × channel × option, storing
it as TSM per-site data under `POOL_FS_<slot>_CH<n>_<option>`.

This replaces the VOH bisection that `CalibrateFullScale` performed inside the rise time steps.
The bisection found the same level indirectly at eight bursts per channel, and had two defects
beyond being ~6× slower: its resolution was ~10 mV against the PPMU's ~1 mV, far too coarse for
the 50 Ω options whose full scale is only ~120 mV; and because one SMU level serves every site it
converged on the lowest site, leaving every other site reading long at 10/90. A PPMU read is
per-site.

An implausible reading — outside 50–150% of the predicted level — is replaced by the prediction
and logged. That window doubles as a relay fault detector: a pin still sitting on the ECL XOR
output reads about −1.0 V or −1.7 V and trips it.

Per-site data is scoped to the semiconductor module context, so **Calibration and MainSequence
must run in the same execution**. Calling Calibration as a sequence before MainSequence satisfies
this; launching it as its own execution does not, and `TryLoad` then returns false and falls back
to the predicted level rather than failing.

Step type must be **Semiconductor Multi Test**, since `PublishPerSite` has no Tests table to
publish into from an Action step. Each published ID needs a matching Tests tab entry, so the full
sweep needs 72. Set `publishResults = false` to run it as an Action instead, storing site data and
logging only.

#### Per-option rise time step

`POOL_RiseTime_Options.PoolRiseTimeByOption(tsmContext, slot, channel, inputOption)` covers all
3 slots × 8 channels × 3 options, publishing three widths and two ratios per combination.
Thresholds come from the stored calibration for that exact combination, so no bisection runs.

One **100 ns timing sheet** serves every option; only the record depth varies, via
`PoolDigital6571Strategy.ConfigureRecord`:

| Option | τ | 90% crossing | Samples @100 ns | Fetch | Margin | Discharge |
|---|---|---|---|---|---|---|
| 50 Ω (both) | 47.62 µs | 109.6 µs | 1,096 | 3,000 | 2.74× | 476 µs |
| 10 kΩ | 909.09 µs | 2093.3 µs | 20,933 | 25,000 | 1.19× | 9.1 ms |

Period and depth are separate knobs and only depth needs to vary, so a second timing sheet buys
nothing and adds a per-option branch plus a sheet to keep in sync — the exact class of silent
mismatch that produced the wrong-capture-bit and wrong-HMOD-chain bugs. `ConfigureRecord` defaults
to the previous 1 µs / 5,000 values, so `SL12`/`SL14`/`SL21_POOL_Check` are unaffected.

Note the SMU threshold tables are **identical across all three slots** — the threshold SMUs sit on
the TFE side of the slot select relays. Only the checker board relays differ, and `PoolPathMap`
derives those (drive 16/18/20, verified against all three literal tables).

Caveats:

- The step constructs `PoolDigital6571Strategy` directly rather than via `PoolCaptureFactory`,
  because the record override is specific to it. The 5172 path is not supported here.
- Changing the timing sheet to 100 ns **invalidates the existing 1 µs sheet for the older
  `SL*_POOL_Check` steps**; they will report widths 10× wrong until they are migrated or given
  their own sheet. `ConfigureRecord` exists so the period is stated explicitly rather than assumed.
- `PoolRcModel.ExpectedWidthSec` still uses the 10 kΩ τ, so limits for the 50 Ω options must come
  from the per-option τ above, not from that property.

#### Test limits

`Reports/add_calibration_limits.py` appends two blocks to `Reports/TestLimits.xlsx`, both for the
10 kΩ option only, and backs up to `.tmp/TestLimits.before_calibration.xlsx` first. It refuses to
run if any published ID already exists, so it is not idempotent-by-overwrite and cannot silently
duplicate rows. Set `OPTIONS = ALL_OPTIONS` to generate the 50 Ω rows; nothing else changes.

| Block | Rows | Test numbers | Tolerance |
|---|---|---|---|
| `POOL_CAL_<slot>_CH<n>_<option>_FullScale` | 24 | 1130–1153 | ±10%, 2045.5–2500.0 mV |
| `POOL_<slot>_CH<n>_<option>_RiseTime_*` and `_Ratio_*` | 120 | 1154–1273 | ±15% widths, ±2% ratios |

The second block is needed because `POOL_RiseTime_Options` publishes different IDs
(`POOL_SL12_CH1_TenKOhm10V_RiseTime_10_90`) from the older `SL*_POOL_Check` steps
(`SL12_POOL_CH1_RiseTime_10_90`). The older 120 rows are left untouched so both step generations
can run; the new rows carry bit-identical limits for the same pairs, since both derive from
τ = 909.09 µs.

Full scale uses ±10% rather than the ±15% applied to widths: tight enough to catch a wrong relay,
a wrong input option or an open path, loose enough not to fail on resistor tolerance, which the
bench data showed to be a few tenths of a percent. It is deliberately tighter than the 50–150%
plausibility window in `PoolCalibrationStore`, which exists to reject an ECL level rather than to
grade the measurement.

#### POOL rise time

Each slot steps a 1 kΩ + 1 µF RC network and times the pulse from the TFE window comparator,
whose two thresholds come from 4163 SMUs and whose XOR output width is the crossing interval.
Three threshold pairs are measured per channel — 10/90, 20/80 and 30/70 — plus the two ratios
between them. The ratios are `ln9 : ln4 : ln(7/3)`, which depend only on the threshold
percentages and so cancel τ, the divider ratio and every component tolerance; a wrong capacitor
passes the ratios and fails the widths, while a broken buffer or comparator fails both.

Analytical expectations and limits are in `Reports/RC_Charge_Times.xlsx`, generated by
`Reports/make_rc_times.py`, and loaded into `Reports/TestLimits.xlsx` as test numbers 1010–1129.
Widths carry ±15% and ratios ±2%.

`captureType` selects the back end: `0` for PXIe-6571 digital capture through the output
changeover's NC branch, `1` for the PXIe-5172 scope through its NO branch. `channel` defaults to
`0`, which sweeps all eight.

##### The TFE input loads the RC node, so τ is the loaded value

The TFE "10 kΩ" input option is named for its input impedance,
`R64 15K ∥ (R55 15K + R66 15K)` = 10 kΩ. That network is the `R64`/`R66` "RESISTOR NETWORK
INPUT" on Tx Board 02-089357 in the CH1 input stage, selected by **HMOD11 K6**, sitting
downstream of the `T_POOL_SL12_OUT_CH_1` test point with nothing buffering it from the checker
board's RC node. So it both loads the node and halves the signal reaching the comparator:

| | Value |
|---|---|
| R input | `R64` ∥ (`R55` + `R66`) = 10 kΩ |
| R effective | 1 kΩ ∥ 10 kΩ = **909.09 Ω** |
| τ | 909.09 Ω × 1 µF = **909.09 µs** |
| V node settled | 5 V × 10k/(1k + 10k) = 4.5455 V |
| V comparator full scale | 4.5455 V × `R66`/(`R55`+`R66`) = **2.2727 V** |

| Pair | Expected width | VOL | VOH |
|---|---|---|---|
| 10/90 | 1.9975 ms | 0.227 V | 2.045 V |
| 20/80 | 1.2603 ms | 0.455 V | 1.818 V |
| 30/70 | 0.7703 ms | 0.682 V | 1.591 V |

A previous revision moved τ to 1.000 ms and the full scale to 2.5 V, on the theory that an
`AD8244` buffer isolated the node. That is wrong, and it does not merely bias the limits — it
breaks the test outright. A 2.5 V full scale puts the 90% threshold at 2.25 V, which is **99% of
the true 2.2727 V asymptote**, so the ramp never crosses it, the comparator window is entered and
never exited, and every width reads `NaN`. The symptom is a captured bit that goes high around
sample 253 and stays high to the end of the record. With the loaded model the 90% threshold is
2.045 V and the pulse ends at 2.3 τ.

The two 50 Ω options (`HMOD11 K4` and `K5`) remain unusable: they load the node to about 47.6 µs
and leave the comparator under 0.25 V full scale, which the window comparator cannot resolve.

The CHMOD6 relay map is verified against sheets 55 and 56: drive `K16`/`K18`/`K20` for
SL12/SL14/SL21, CH1 fan-out at `K17`/`K19`/`K21`, then CH2–CH8 in steps of 3 from `K22`/`K23`/`K24`.

##### The drive path needs HMOD14 K5/K6/K7

Tx Board sheet 31, "PXIE-6571 To HSD200(SLOT11) CONNECTIONS", routes each 6571 DIO to its HSD200
channel through `HMOD14 Kn`, numbered channel for channel: `DIO_4 → CH5` via **K5**,
`DIO_5 → CH6` via **K6**, `DIO_6 → CH7` via **K7**. Without them the pattern never reaches
`T_HSD200_SL11_CH5/6/7`, so the checker board's `K16` sees no step, the node stays at 0 V and
every width is `NaN`. This fails misleadingly, because the comparator chain is untouched:
`DIO_24` still shows the XOR output at −1.73 V and still tracks the thresholds.

`K1–K4` are the `DIO_0..3 → CH1..CH4` legs, not the shift-register path — the HMOD and CHMOD
chains use the dedicated `HMOD_DIN` / `CHMOD_DIN` / `CHMOD_RESET` pins. They are left closed to
match the BBAC steps. Note `HMOD14to18` is a full-chain overwrite, so this write also drops
`K12–K15` and `K21–K26` from `grp_xptsw_pins_hmod14`, which `HMODInitialization` sets.

##### Capture samples are packed MSB first

`DIO_24..DIO_31` occupy one 8-bit parallel sample with the **first pin in the waveform's pin list
as the high bit**, so CH1 (`DIO_24`) is bit 7 and CH8 (`DIO_31`) is bit 0. The bit index is
therefore `PoolCapturePins.Dio.Length - _channel`, not `_channel - 1`. The latter masks `0x01`
for CH1, which never matches the `0x80` the hardware sets, so `PulseWidth` sees no high sample
and returns `NaN` on every channel — indistinguishable from a signal that never arrives.

##### Thresholds are calibrated per channel, not taken from the nominal full scale

`CalibrateFullScale` bisects VOH to measure the settled voltage at the comparator before the
30-70 pair is measured, and the thresholds are then fractions of that measured level via
`ThresholdPair.LowVoltsFor` / `HighVoltsFor`. The probe needs no extra instrument: with VOL held
at 0.05 V the window closes only if VOH is below the settled level, so "did every site return a
finite width" is itself a comparator for VOH against that level. Eight steps narrow 0.5–3.0 V to
under 10 mV. The lowest level across sites is used, since one SMU level serves every site.

This is necessary because a crossing time depends on `ln(1 − V/A)`, which is brutally sensitive
near the asymptote. A 3% error in the settled level `A` moves the 90% crossing from 2.30 τ to
2.63 τ — a 16% error in the 10/90 width — while costing 10% at 20/80 and 8% at 30/70. First
silicon showed exactly that ranking:

| Pair | Implied τ (mean of 8 channels) |
|---|---|
| 30/70 | ~910 µs — matches the model's 909.09 µs |
| 20/80 | ~978 µs |
| 10/90 | ~1075 µs |

The least sensitive pair confirmed τ, and the inflation grew with sensitivity, which located the
error in `A` rather than in τ. Solving back gave `A` ≈ 2.195 V against a nominal 2.2727 V, about
3.4% low — consistent with drive droop into 909 Ω plus resistor tolerance. With absolute
thresholds every channel failed 10/90 while passing 20/80 and 30/70; calibrated thresholds remove
the drive droop and the 1 kΩ and 15K tolerances in one step.

##### The longest pulse is measured, not the first

`LongestHighRun` scans the whole record and returns the widest complete high run. Taking the
first run lets a narrow glitch ahead of the real pulse win and *truncate* the result, which was
observed as 1 µs and 2 µs widths on 2 of 24 channels, producing ratios of 2378 and 0.5. A run
still high at the end of the record is ignored, since it has no falling edge and so no measurable
width — that preserves the `NaN` contract instead of silently reporting the distance to the end of
the capture.

##### Digital capture needs the `capture` opcode, not just the `V` pin state

`V` only marks a pin as capturable. The per-vector `capture` opcode is what takes the sample.
With `V` alone the burst succeeds and `FetchCaptureWaveform` then fails with
`Samples Available: 0`, even though the `capture_start`/`capture_stop` bracket, the burst label,
the waveform's Parallel mode and pin list, the comparator levels and the 0.1 µs strobe are all
correct. Bursting the pattern directly in the Digital Pattern Editor reproduces the zero, which
is what localises the fault to the pattern rather than to the C#.

Since only the repeated vector carries `capture`, the record holds two samples fewer than the
repeat count and t = 0 sits at vector 2 rather than vector 1. The one vector offset does not
affect a width, which is a difference between two crossings inside one record.

##### Record depth is set by the measured 90% crossing, not the nominal one

`PoolRecordGeometry.SamplesToFetch` returns **49000** samples for the 10 kΩ option, which needs
the pattern repeat count to be at least 49000. All four `ConfigureRecord` call sites — the three
`SL*_POOL_Check` steps and `POOL_RiseTime_Options` — route through it, so the depth is set in one
place.

The earlier 26000 was derived from the nominal 2.303 τ crossing at 20933 samples plus allowance
for a capacitor 24% high. Raw captures of all eight SL12 channels disproved that margin:

| Channel | 90% crossing | Margin to the old 26000 sample window |
|---|---|---|
| CH6 | 2599.3 µs | 0.7 µs |
| CH1 | 2592.7 µs | 7.3 µs |
| CH8 | 2589.4 µs | 10.6 µs |
| CH2 | 2565.3 µs | 34.7 µs |
| CH4 | 2549.9 µs | 50.1 µs |
| CH7 | 2484.3 µs | 115.7 µs |

CH1, CH6 and CH8 are exactly the channels that returned an intermittent `NaN` on the 10/90 pair.
The cause is the window edge, so the symptom is not distinguishable from a dead channel by
looking at the result alone — which is the reason the depth is now sized from measurement.

The crossings run late because the 10/90 high threshold at 0.9 full scale sits only 60–200 mV
below the asymptote the node actually reaches, so the signal creeps across it on the flattest
part of the ramp. Raising the depth stops a crossing that *does* occur from being truncated; it
does not address the threshold margin itself, which is a question about the drive level. Fitting
τ and the asymptote to the three glitch-immune widths per channel gives a true final value of
2.11–2.25 V against the calibrated 2.2727 V, and a τ spread of 724–1000 µs that a single shared
1 kΩ/1 µF cannot explain.

Because of that margin, **30/70 is the most trustworthy of the three pairs** — it sits in the
steepest part of the ramp, so it is insensitive both to the asymptote and to the comparator
chatter below. 10/90 is structurally fragile on this hardware regardless of record depth. This is
why 30-70 is now the only pair measured; see the threshold-pair section above.

##### The V(t) sweep step

`POOL_VtSweep.PoolNodeVoltageSweep(tsmContext, slot, channel, inputOption, publishResults)`
samples the RC node against time so the asymptote, τ and shape are measured rather than inferred.
Defaults are `slot = 0, channel = 0, inputOption = 2` — every slot and channel, matching the
calibration and rise-time steps, so the **312 published IDs line up exactly with the 312 generated
limit rows**. That alignment is not cosmetic: a Semiconductor Multi Test step errors on any Tests
tab entry that is never published, so a narrower default fails the step for every combination it
skipped. Pass `slot = 12, channel = 1` for a single combination on the bench.

The full sweep costs about **4.5 s** — 24 combinations at roughly 190 ms each, being eleven
discharges of 10 τ (100 ms) plus 42 τ of delays (38 ms). The discharges dominate, not the delays.

It exists because three window widths cannot distinguish the two candidate defects. Fitting the
measured SL12 widths gives an **identical 2.31% residual** for a reduced asymptote of 0.947 full
scale and for a constant threshold offset of +121 mV, both wanting τ ≈ 832 µs against the
modelled 909 µs. That degeneracy is structural, so no number of parts separates them.

The threshold half of that pair is now ruled out on the bench. The SMU forces the window edges at
0.2274 V and 2.0466 V, exactly 10% and 90% of the 2.274 V calibration stored for the channel, and
reads them back to eight decimals — so the calibrate-to-force chain is sound. It sources 4.3 and
4.5 µA, which raised the prospect of an I×R error to the comparator, but the current is **sourced**
rather than sunk, giving the drop the wrong sign to lengthen a width, and the channels are
**remote sensed**, so it is regulated out regardless. That leaves the asymptote.

###### Threshold pairs: 30-70 only

`POOL_RiseTime_Options` measures **one** threshold pair, `30_70`, selected by name via
`MeasuredPairName` so that reordering the shared `PoolRcModel.Pairs` cannot silently change it.
10-90 and 20-80 are retired, and with them both published ratios, since each ratio needs two
pairs.

The reason is amplitude error gain: a 1% asymptote shortfall moves the 10-90 width by **5.79%**
against 30-70's **2.55%**, because 10-90's high threshold sits only 227 mV below an asymptote the
ramp may not reach while 30-70 sits in the steepest part of the ramp with 682 mV of headroom.
10-90 failed 40 of 40 results while 30-70 passed 40 of 40 from the *same* captures and the *same*
calibration. Dropping the two pairs saves only about 1.6 s plus 48 fetches across the whole
block, so this was a correctness decision, not a throughput one.

What this costs: the ratios were the only **τ-independent shape monitor** in production. Shape is
now covered only by `POOL_VtSweep`, which is a characterisation step. If a future failure needs
shape data, it will have to be collected deliberately rather than read out of history.

Published IDs are now `POOL_<slot>_CH<n>_<option>_RiseTime_30_70` — 3 options × 24 channels = **72
rows**, plus 72 calibration rows, 144 total reconciled exactly against the limits file.

###### V(t) sweep delays

Delays are multiples of the option's own τ, so one list serves all three options despite their
twentyfold spread: 0.55, 1.1, 1.65, 2.2, 3.3, 4.4, 6.6, 8.8, 13.2 τ. Published IDs are
`POOL_VT_<slot>_CH<n>_<option>_Tau055` … `_Tau1320`, plus `_Asymptote` and `_TauEstimate`.

Three constraints are load bearing:

| Constraint | Value | Why |
|---|---|---|
| Aperture | **20 µs** | Calibration's 2 ms is right for a settled node but integrates 2.4 τ on a ramp and smears the curve flat. This is the one constant that must not be copied from that step. |
| PPMU force | **0 A** | The node sits behind the 15K divider at ~7.5 kΩ, so forcing voltage loads it. |
| Discharge | **10 τ** before every point | Without it a point starts from where the previous longer delay left the capacitor, and the curve flattens into the asymptote from the first point while still looking like data. |

Delays start at 0.55 τ rather than near zero because `Globals.TheHdw.Wait` is a software wait with
millisecond-scale jitter, so points below ~1 τ carry timing error comparable to the delay. That
costs nothing, because both unknowns live in the tail; the 13.2 τ point is settled to within 2 ppm
and is published as the asymptote directly rather than as an estimate. τ is estimated by inverting
V = A(1 − e^−t/τ) at the point nearest 1 τ, where sensitivity is highest, and returns `NaN` for a
site that cannot be inverted rather than a plausible-looking number.

**What it cannot settle.** The sweep drives with `WriteStatic`, like calibration, so it measures
the static path. The rise-time step's step is driven from the *pattern* — `StepAndMeasure` only
calls `WriteStatic(PinState._0)` before bursting, and there is no `WriteStatic(PinState._1)` in the
digital strategy at all. So if the burst reaches a different level than static drive, this sweep
will not show it. That question is answered by checking the state the drive pins
`T_HSD200_SL11_CH5/6/7` hold on the repeated capture vector in the DPE; the timing sheet puts all
11 pins, drive included, in `tset1`, so their per-vector state matters.

##### Glitch immunity is already in the width measurement

`PulseWidth` calls `PoolPulse.LongestHighRun`, which differences the edge indices of the longest
contiguous high run rather than counting high samples or spanning first rising to last falling
edge. That matters because comparator chatter is heavy and strongly channel dependent. High run
counts from the raw captures, per 10/90, 20/80, 30/70:

| Channel | High runs | Verdict |
|---|---|---|
| CH1, CH4, CH7 | 1 / 1 / 1 | clean |
| CH2, CH8 | 1–2 | clean |
| CH6 | 35 / 9 / 5 | mild |
| CH3 | 25 / 31 / 33 | mild |
| CH5 | 447 / 182 / 119 | severe |

The glitches are small: CH5's 447 runs total 2522 µs against a longest run of 2431 µs, so about
446 glitches averaging ~0.2 µs, roughly two samples each. The pulse body is intact, so this is
chatter at the crossings and not a mid-pulse dropout. CH5's chatter extends to 3024 µs, after its
main run ends, consistent with the signal hovering at a threshold close to the asymptote. Taking
the longest run rather than the first-to-last span is what keeps CH3 and CH5 measurable at all.

##### The 5172 needs an immediate trigger

The scope's default edge trigger sits at 0 V, which the NECL XOR output terminated 50 Ω to −2 V
never reaches — it swings about −1.7 V to −0.9 V, so the trigger never fires. `Configure` calls
`ConfigureTriggerImmediate` on each session directly, because the `InstrCtrl.Scope` wrapper only
exposes a digital edge trigger. `StepAndMeasure` arms before driving the step and the 12 ms
record is twice the ramp, so the pulse is contained without an aligned trigger.

An unconnected 1 MΩ scope input floats near 0 V, which is *above* the ECL midpoint, so
`PulseWidth` sees a signal that starts high and never falls and returns `NaN` — the same result
as a real but absent pulse. The per-fetch min/max is logged to separate the two: ECL reads about
−1.7 to −0.9 V, a floating input reads near 0 V on both.

##### The output changeover selects the source, not the destination

`K7`/`K16`/`K25`… must stay **de-energised** for both back ends. The XOR output lands on the
changeover's `B_NC` contact, and its common feeds both taps, so the destination is chosen by
closing the scope tap or the digital tap. Energising the changeover switches the common onto
`B_NO` and disconnects the XOR output entirely.

#### DIFFMETER limits

Each checker forces 10 V on the odd DC30V channel and 9 V on the even one, so the differential
reading is +1 V in the normal relay orientation and -1 V reversed. `Reports/TestLimits.xlsx`
carries test numbers 887-952 for the ten channel pairs per slot plus the two `HMOD13_METER1` /
`HMOD13_METER2` final checks: `_POS` and `HMOD13_METER*` limits are 0.95 to 1.05 V, `_NEG` limits
are -1.05 to -0.95 V.

The Pin column is empty for every DIFFMETER row: all three checkers publish with
`tsmContext.PublishPerSite`, which takes no pin. The reading comes from `P131_DIFF_METER` for the
`Meter1Channels` pairs and the `HMOD13_METER1` check, and from `P131_4081_DMM` for the remaining
pairs and the `HMOD13_METER2` check.
| SL02_BBAC_SRC_Check | Slot 2 BBAC source checker for CH1 (`SL02BBACSrcCh1Check`) and CH2 (`SL02BBACSrcCh2Check`) |
| SL02_BBAC_CAP_Check | Slot 2 BBAC capture checker for CH1 (`SL02BBACCapCh1Check`) and CH2 (`SL02BBACCapCh2Check`) |

#### SL02_BBAC_SRC_Check

Exercises the Slot 2 BBAC source path: PXIe-4467 AC source → Tx Board routing (02-089357)
→ Checker Board bridge rectifier and DIB access network (02-101092 sheet 53).

Checks performed per channel, in order:

1. Bridge rectifier output on METER_HI/METER_LO with a 1 kHz 10 Vpp sine on SRCx+/- (1.6 kHz LPF).
2. SRCout+/- measured with the PXIe-4163 PPMU.
3. SRCxREF connectivity on METER_HI with METER_LO grounded through RL0.
4. Common-mode path: PXIe-4147 forces +1 V, read back on the PXIe-4163 ACC_SRCxREF channel.
5. DIB access of ACC_SRCxREF, ACC_SRCx- and ACC_SRCx+ (+1 V forced, measured on METER_HI).
6. DGSsrcx reference: each DIB access net forced to +2 V reads +1 V against the 1.0 V ADR130 reference.
7. cc common point: ACC_SRCx+ at +1 V reads +1 V against AGND, +2 V once cc common moves to DGSsrcx.
8. DGSsrc functionality: each DIB access net switched onto DGSsrcx reads the 1.0 V reference.

Relay mapping. The BBAC Source Checker block diagram uses mnemonic names that do not match the
schematic designators, so the code is written against the schematics: Tx Board 02-089357 sheet 74
(CH1) / sheet 75 (CH2), Checker Board 02-101092 sheet 53.

| Block diagram | Function | CH1 | CH2 |
|---|---|---|---|
| KA / KB | 4467 AOUT+ → SRCx+ (499 R / 3300 pF LPF) | HMOD18 K29 / K31 | HMOD23 K23 / K28 |
| KG / KH | 4467 AOUT- → SRCx- | HMOD18 K30 / K32 | HMOD23 K24 / K29 |
| KD / KE | 49.9 R divider → SRCxREF branch | HMOD23 K17 / K18 | HMOD23 K25 / K26 |
| KF | changeover: closed = COMMON MODE/DC OFFSET, open = SRCxREF | HMOD23 K19 | HMOD23 K27 |
| KV | SRCx- rail → ACC_SRCx- row | HMOD23 K20 | HMOD23 K30 |
| KS | SRCxREF / common-mode rail → ACC_SRCxREF row | HMOD23 K21 | HMOD23 K31 |
| KC | SRCx+ rail → ACC_SRCx+ row | HMOD23 K22 | HMOD23 K32 |
| KI / KJ | 4163 → ACC_SRCx- row → ACC_SRCx- pin | HMOD24 K65 / K66 | HMOD25 K65 / K66 |
| KT / KU | 4163 → ACC_SRCxREF row → ACC_SRCxREF pin | HMOD24 K67 / K68 | HMOD25 K67 / K68 |
| KK / KL | 4163 → ACC_SRCx+ row → ACC_SRCx+ pin | HMOD24 K69 / K70 | HMOD25 K69 / K70 |
| KM / KN / KO | ACC_SRCx- / REF / + row → DGS_SRCx | HMOD24 K61 / K62 / K63 | HMOD25 K61 / K62 / K63 |
| KP | cc common → DGS_SRCx | HMOD24 K64 | HMOD25 K64 |
| KQ | 4147 LO changeover: open = AGND, closed = cc common | HMOD13 K31 | HMOD13 K32 |
| KR | 4163 LO sense changeover: open = AGND, closed = CC COMMON2 | HMOD13 K22 | HMOD13 K22 |
| K1 | bridge rectifier output → METER_HI / METER_LO | CHMOD1 K15 | CHMOD1 K18 |
| K7 | SRCx+/- changeover: open = bridge, closed = BBAC CAPx test points | CHMOD1 K13 | CHMOD1 K16 |
| K4 | DGS_SRCx changeover: open = AGND, closed = 1.0 V BBAC ref | CHMOD1 K14 | CHMOD1 K17 |
| K2 | SRCxREF → METER_HI | CHMOD5 K68 | CHMOD6 K1 |
| K9 | ACC_SRCx- → METER_HI | CHMOD5 K69 | CHMOD6 K2 |
| K3 | ACC_SRCxREF → METER_HI | CHMOD5 K70 | CHMOD6 K3 |
| K6 | ACC_SRCx+ → METER_HI | CHMOD5 K71 | CHMOD6 K4 |
| K8 | DGS_SRCx → METER_LO | CHMOD5 K72 | CHMOD6 K5 |

Notes:

- The slides' `K5` (ACC_SRCx- → METER_HI) is `K9` on the block diagram and CHMOD5 K69 / CHMOD6 K2
  in the schematic.
- K7 and K4 are changeover relays whose normally closed contacts are the useful default, so K7 is
  held open for the whole check and K4 is only energised for the 1.0 V referenced measurements.
- The 1.0 V BBAC reference is generated on the Checker Board by U15 / U17 (ADR130AUJZ) from 15V_1_HI.

Instrument resources (added to `TBChecker.pinmap`):

| Pin | Instrument | Channel |
|---|---|---|
| `P122_4467_SRC_AO0` / `AO1` | `DSA_4467_C1_S15_AOFG_0` / `_AOFG_1` (DAQmx AOFuncGen tasks) | `ao0` / `ao1` |
| `P138_4163_SMU_CH18`–`CH23` | `SMU_4163_C2_S13` | 18–23 |
| `P103_4147_SMU_CH0` / `_CH1` | `SMU_4147_C2_S16` | 0 / 1 |

The PXIe-4467 is a DAQmx DSA device, not an NI-FGEN device, so the AC source runs through DAQmx
`AOFuncGen` tasks (`DSA_4467_C1_S15_AOFG_0` / `_AOFG_1`). Opening it with `NIFgen` raises
`IVI 0xBFFA0060 Insufficient location information or resource not present in the system`.
`InstrCtrl.SetDAQmxAOFuncGenTasks` creates the tasks, then `ConfigureAOFuncGen` /
`StartAOFuncGen` / `Stop` drive them. Amplitude is specified zero-to-peak, so the 10 Vpp
stimulus is passed as 5 V.

Limits are in `Reports/TestLimits.xlsx` (test numbers 953-984, prefix `SL02_BBAC_SRC_*`). DC
referenced steps use +/-0.05 V around the forced value. Three IDs per channel still need a
designer-supplied expected value and currently carry only a sanity envelope:
`_BridgeRect_V` (0-15 V), and `_SrcOutN_V` / `_SrcOutP_V` / `_SrcRef_V` (+/-5.5 V, since a DC
sample of the live 10 Vpp sine can land anywhere in that range).

Only the three PXIe-4163 readings carry a Pin: `_SrcOutN_V` (`P138_4163_SMU_CH18` / `CH21`),
`_SrcOutP_V` (`CH20` / `CH23`) and `_CommonMode_V` (`CH19` / `CH22`). Every other source ID goes
through `IMeterStrategy.PublishResult`, which uses `tsmContext.PublishPerSite` and takes no pin.

#### SL02_BBAC_CAP_Check

Exercises the Slot 2 BBAC capture path: the BBAC source relays plus the source-sheet `K7` divert
the PXIe-4467 AC source onto the BBAC CAPx test points, the Checker Board capture relays feed them
into CAPx+/-, and the Tx Board routes CAPx+/- back to the PXIe-4467 differential analog input.
Entry points `SL02BBACCapCh1Check` / `SL02BBACCapCh2Check`.

Steps, in order:

1. AIN functionality — 1 kHz 10 Vpp sine through CAPx+/-, peak-to-peak measured on the 4467
   differential input (`_AinDiff_Vpp`).
2. CAPx+/- pogo — PXIe-4147 CH2 / CH3 measure CAPx+ and CAPx- against AGND through the column
   relays KI / KJ (`_CapP_V`, `_CapN_V`).
3. PPMU to DIB access — 4147 forces +1 V onto ACC_CAPx+/-, `K1` then `K2` tap each net onto
   METER_HI (`_AccCapP_V`, `_AccCapN_V`).
4. DGSCap / cc common1 — `KM` ties CC COMMON1 to DGSCapx; with `K3` closed the ACC_CAPx- reading is
   +2 V, with `K3` open it is +1 V (`_AccCapN_DgsRef_V`, `_AccCapN_DgsGnd_V`).
5. DGSCap connectivity — `KN` then `KO` switch the 1.0 V referenced DGSCapx onto ACC_CAPx+ and
   ACC_CAPx- (`_DgsFunc_AccCapP_V`, `_DgsFunc_AccCapN_V`).

Relay mapping (block diagram mnemonic → schematic designator):

| Mnemonic | Function | CH1 | CH2 |
|---|---|---|---|
| KA / KB | 4467 AIN+ onto CAPx+ network / onto the Slot 2 pin | HMOD4 K28 / K30 | HMOD10 K31 / HMOD13 K23 |
| KC / KD | 4467 AIN- onto CAPx- network / onto the Slot 2 pin | HMOD4 K29 / K31 | HMOD10 K32 / HMOD13 K24 |
| KE / KF | 4147 CH2 onto ACC_CAPx+ row / onto the Slot 2 pin | HMOD4 K32 / HMOD8 K29 | HMOD13 K25 / K26 |
| KG / KH | 4147 CH3 onto ACC_CAPx- row / onto the Slot 2 pin | HMOD8 K30 / K31 | HMOD13 K27 / K28 |
| KI / KJ | CAPx+ rail to ACC_CAPx+ row / CAPx- rail to ACC_CAPx- row | HMOD8 K32 / HMOD10 K29 | HMOD13 K29 / K30 |
| KK / KL | 4147 shared LO changeover: closed = CC COMMON1, open = CC COMMON2 | HMOD13 K31 | HMOD13 K31 |
| KM | CC COMMON1 onto DGSCapx | HMOD4 K27 | HMOD10 K30 |
| KN / KO | ACC_CAPx+ / ACC_CAPx- row onto DGSCapx | HMOD24 K71 / K72 | HMOD25 K71 / K72 |
| K3 | DGSCapx changeover: open = AGND, closed = 1.0 V BBAC ref | CHMOD1 K19 | CHMOD1 K20 |
| — | CAPx+ / CAPx- onto the SRCx+ / SRCx- test points | CHMOD6 K6 / K7 | CHMOD6 K11 / K12 |
| K1 / K2 | ACC_CAPx+ / ACC_CAPx- onto METER_HI | CHMOD6 K8 / K9 | CHMOD6 K13 / K14 |
| K4 | DGSCapx onto METER_LO (not used; METER_LO is grounded via RL0) | CHMOD6 K10 | CHMOD6 K15 |

Notes:

- Both capture channels share PXIe-4147 CH2 and CH3, confirmed against 02-089357 sheet 76 (CH1)
  and sheet 77 (CH2). `CH2_HI` reaches `ACC_CAP1+` through HMOD4 K32 / HMOD8 K29 and reaches
  `ACC_CAP2+` through HMOD13 K25 / K26; `CH3_HI` reaches `ACC_CAP1-` through HMOD8 K30 / K31 and
  `ACC_CAP2-` through HMOD13 K27 / K28. Only one capture channel may be driven at a time, so
  `SL02BBACCapCh1Check` and `SL02BBACCapCh2Check` must not be run concurrently.
- The 4147 LO is a single node, `P103_4147_SMU_CH0-CH3_LO`, shared by all four channels and switched
  by one relay, HMOD13 K31 (sheet 74). The earlier HMOD4 K25 / K26 mapping was wrong. The checker
  leaves this relay alone because it also grounds the two BBAC source channels, and because the
  shared LO means the +5 V DIB rails on 02-084291 could not be stacked.
- `SetDAQmxAITasks` gained optional `minimumVoltage` / `maximumVoltage` parameters (defaults
  unchanged at ±1 V) because the 10 Vpp stimulus would otherwise clip the ±1 V AI range.

Instrument resources added to `TBChecker.pinmap`:

| Pin | Instrument | Channel |
|---|---|---|
| `P122_4467_SRC_AI0` / `AI1` | `DSA_4467_C1_S15_AI_0` / `_AI_1` (DAQmx AI tasks) | `ai0` / `ai1` |
| `P122_4467_SRC_AO0` / `AO1` | `DSA_4467_C1_S15_AOFG_0` / `_AOFG_1` (DAQmx AOFuncGen tasks) | `ao0` / `ao1` |
| `P103_4147_SMU_CH2` / `_CH3` | `SMU_4147_C2_S16` | 2 / 3 |

Limits are in `Reports/TestLimits.xlsx` (test numbers 985-1002, prefix `SL02_BBAC_CAP_*`). `_AinDiff_Vpp` (0-11 V) and
`_CapP_V` / `_CapN_V` (+/-5.5 V) carry sanity envelopes only: the expected peak-to-peak amplitude
depends on the capture front-end gain and needs a designer-supplied number.

Only the two PXIe-4147 readings carry a Pin: `_CapP_V` (`P103_4147_SMU_CH2`) and `_CapN_V`
(`P103_4147_SMU_CH3`). `_AinDiff_Vpp` and the METER_HI readings publish per site with no pin.

### P1Checker (Phase 1 — migrated from checkerboard)

| Module | Description |
|--------|-------------|
| P1_SupplyCheck | Measures 12 on-board supply voltages via DMM through HMOD18 relay mux |
| P1_HSDOpenCheck | Verifies HSD200 relay group connectivity via PPMU force/measure |
| P1_DC30OpenCheck | Verifies DC30V channel connectivity (FIMV odd, FVMI even) |
| P1_DC90OpenCheck | Verifies DC90V channel connectivity and on-board relay droop |
| P1_POOLOpenCheck | Verifies DIO POOL path connectivity through HMOD11-13 |
| P1_WindowComparatorCheck | Validates AD96687BRZ window comparator via VOH/VOL thresholds |
| P1_HMODRelayCheck | Measures 12V relay supply droop across all 22 HMOD groups |

### HMOD Control

| Method | Chain | Description |
|--------|-------|-------------|
| HMOD1to4 | 32-bit × 4 | Tx Board DC30 relay matrix |
| HMOD5to10 | 32-bit × 6 | DGS relay routing |
| HMOD11to13_24to25 | 32-bit × 3 + 72-bit × 2 (240 bits) | HMOD11–13 daisy-chained with HMOD24–25 for differential meter routing |
| HMOD14to18 | 32-bit × 5 | HSD/SPI relay routing |
| HMOD19to23 | 32-bit × 5 | Additional Tx Board groups |
| CHMOD1to6 | 32-bit × 1 + 72-bit × 5 (392 bits) | Checker Board HMODs |

### Relay name validation

`RelayID72` throws `ArgumentOutOfRangeException` for anything outside K1–K72. It previously
dropped out-of-range relays silently and returned a well-formed all-zero mask, so a caller that
computed a bad relay number still got a valid result and the measurement ran with no relay closed
instead of failing. `SL10_DIFFMETER_Check.FinalCheck` did exactly that: it asked for K73/K76 and
reported a meter reading of nothing.

### HMOD24/25 relay remap

`RelayID72` takes an opt-in `applyHmod2425Remap` flag, which routes the relay number through
`RemapHmod2425Relay`. HMOD24 and HMOD25 are miswired between K21 and K56 in four contiguous
blocks: K21–K28 shift up by 4, K29–K36 up by 20, K37–K52 down by 4, and K53–K56 down by 32.
Everything outside that range passes through unchanged, and K57–K60 are confirmed correct because
they are the only relays SL10 channel pairs 5 and 10 use and those are the only SL10 pairs that
pass. The flag is off by default so CHMOD callers, which are wired correctly, are unaffected.

### Channel pair to relay block

Each DIFFMETER step owns one `RelayOffsetFor(chPair)` that maps a channel pair to its relay
block, and `SL10_DIFFMETER_Check` adds `UsesHmod25(chPair)` for its 5/5 split across HMOD24 and
HMOD25. Both the per-channel loop and `FinalCheck` go through these, so they cannot disagree.
Hand-written offset expressions in `FinalCheck` were the source of three separate defects: an
offset of 72 that requested relays above K72, a pair-9 mask sent to HMOD24 when pair 9 lives on
HMOD25, and SMU channels that the selected relays do not connect.

### Meter path verification

`METER1` and `METER2` are separate differential buses on the Translator Board
(`SL*_DCDIFF_METER1_HI/LO` and `SL*_DCDIFF_METER2_HI/LO`). The two HMOD13 relays that select a
meter are independent two-pole devices, each switching its own DMM's HI and LO onto its own bus:

| Step | METER1 relay → DMM | METER2 relay → DMM |
| --- | --- | --- |
| `SL04` | K12 → slot 16 | K15 → slot 13 |
| `SL10` | K13 → slot 16 | K16 → slot 13 |
| `SL24` | K14 → slot 16 | K17 → slot 13 |

Because a channel pair's HMOD24/25 relay block lands on only one of the two buses, the meters
cannot both read the same pair. `FinalCheck` originally closed both meter taps against a single
pair, which left METER2 correctly connected to a bus with nothing driven onto it, reading a few
millivolts. It now calls `MeasureMeterPath` once per meter, using the pair that feeds that
meter's bus — pair 9 for METER1 and pair 10 for METER2, per `Meter1Channels`. The helper derives
its SMU pins from the pair (`chOdd = chPair * 2 - 1`) so the forced channels always match the
relays that are closed.

### SMU LO sense and the DGS 1 V shift

Several BBAC tests assert that a net forced to +1 V reads +2 V once its DGS node is referenced to
the 1.0 V BBAC reference. That shift only appears if the forcing SMU's **LO sense** follows the
node being lifted. All BBAC channels are configured for `DCPowerMeasurementSense.Remote`, so the
LO sense line — not the LO force line — sets the regulation reference:

| Step | Relay | Function |
| --- | --- | --- |
| `SL02_BBAC_SRC` | `MaskKr` = HMOD13 K22 | shared PXIe-4163 LO sense: open = CC COMMON1, closed = CC COMMON2 |
| `SL02_BBAC_CAP` | `LoChangeover` = HMOD13 K31 | PXIe-4147 CH0-CH3 LO onto CC COMMON1 |

K22 is a **CH1/CH2 selector**, not an AGND changeover as its original comment claimed. CH1 needs
it open and CH2 closed, encoded by a per-channel `MaskKr` that is `0` for CH1. Driving it for both
channels simply moved the failure: `CcCommon_DGS` read 1.999 on CH2 and 0.9999 on CH1, an exact
swap of the original symptom.

Both were previously left open — `MaskKr` explicitly, `LoChangeover` by never being referenced at
all despite being declared for both channels. With the LO sense pinned to AGND the shift is
physically impossible regardless of the KQ/KP/K4 (SRC) or KM/K3 (CAP) relays, so the affected
tests returned the forced +1 V and matched their AGND counterparts to four digits in every run.

`MaskKr` is deliberately open for the rest of the SRC step, so it is closed only around the
`CcCommon_DGS` reading and released immediately after. KQ and KR are both on HMOD13 and are set in
a single `Apply`, as the HMOD write semantics below require.

### CC COMMON1 and CC COMMON2 are star-grounds released by HMOD13 K31 / K32

The Translator Board notes that "CC COMMON1 and CC COMMON2 are star-ground". HMOD13 K31 and K32 are
**normally closed to AGND**, so each must be energized to release its node before that node can be
lifted off ground. The relay is per channel and only one may be driven at a time:

| Channel | Releases | `LoChangeover` | `LoSenseTie` |
| --- | --- | --- | --- |
| CAP CH1 | CC COMMON1 | HMOD13 K31 | none needed |
| CAP CH2 | CC COMMON2 | HMOD13 K32 | HMOD4 K25 + K26 |

`SL02_BBAC_SRC` uses the same pair as its per-channel `MaskKq` (K31 for CH1, K32 for CH2) and its
`CcCommon_DGS` passes at 1.999 on both channels, so this is the proven mechanism.

Both CAP channels share the PXIe-4147 CH2/CH3 PPMUs, and only the LO **force** is ganged across
CH0-CH3 (`P103_4147_SMU_CH0-CH3_LO`); the LO **sense** lines are per channel. Per sheet 76, HMOD4
K25/K26 switch those sense lines with `A_NO` on CC COMMON2 and **`A_NC` on CC COMMON1**:

- CH1 needs no relay — the sense lines rest on CC COMMON1, its own reference (`LoSenseTie` is null).
- CH2 must drive K25/K26 for its **entire run**. Releasing them mid-run reverts the sense lines to
  CC COMMON1, leaving CH2 measuring against the other channel's reference.

### Both CAP PPMUs must source during the DIB access check

`CheckPpmuToDibAccess` closes KE/KF to put PXIe-4147 CH2 on ACC_CAPx+ and KG/KH to put CH3 on
ACC_CAPx-, and the procedure expects **+1 V on both** nets. Only CapPlus used to be forced, leaving
ACC_CAPx- floating on residual charge: `AccCapN` read 1.5 V to 1.7 V on both channels and drifted
between runs while `AccCapP` held steady at 0.9998. Both PPMUs now force +1 V and are returned to 0 V.

### Every meter read settles first

`LoChangeover` (HMOD13 K31 for CH1, K32 for CH2) is the opposite case and must **not** be given channel
scope. K31/K32 are the AGND star-ground for CC COMMON1/CC COMMON2, so energising one lifts that cc
common off ground. It is therefore closed and released together with K3, the only window in which the cc
common is held at BBAC_1V_REF. Holding it any longer leaves the cc common ungrounded for every other
measurement in the step. Note `CheckDgsCapConnectivity` also closes K3, but there KM is open so K3
references only DGSCapx and not the cc common, which is why `LoChangeover` is correctly absent from it.

KR (HMOD13 K22) is now held for the whole SRC channel instead of being closed and released inside
`CheckCcCommonPoint`. K22 routes the shared PXIe-4163 CH18-CH23 LO sense, and its rest position is CC
COMMON1. Because `CheckCcCommonPoint` runs *after* `CheckDgsSrcReference`, CH2 measured
`DgsRef_AccSrcRef` with that sense still on CH1's cc common, so the 4163 regulated against the wrong
node. Only that one reading was affected: `DgsRef_AccSrcN` and `DgsRef_AccSrcP` use other SMUs and both
read 1.000. CH1's `MaskKr` is 0, so it never touches K22 and stays on CC COMMON1 as required. This is
the same class of bug as CAP's `LoSenseTie`: a shared sense selector must span the whole channel, not
one function.

Swapping the meter proved how much this mattered. With the PXIe-4139 the step reported 3 failures; with
the PXIe-4081, 14 — including `BridgeRect` CH1 at **20.54 V** against 4.895 V, over its 15 V limit, and
`AccSrcRef` at 0.8341 V against 0.9992 V. Unsettled reads track the instrument, not the board. Five
measurements published with no preceding wait and all now have one: `BridgeRect`, `AccSrcRef`,
`CcCommon_DGS`, `MeasureDgsNetOnMeter` (SRC) and `MeasureOnMeter` (CAP).

`SL02_BBAC_CAP.MeasureOnMeter` and `SL02_BBAC_SRC.MeasureDgsNetOnMeter` closed the METER_HI tap and
read immediately, unlike every other measurement in both steps. `CheckDgsCapConnectivity` had no wait
of its own either, so its first reading was taken while the net was still discharging from the +2 V
left by `CheckDgsCapCcCommon` toward the +1 V reference: `DgsFunc_AccCapP` on CH2 landed anywhere
between 1.60 V and 2.10 V and moved every run, while the later `AccCapN` read passed only because the
intervening relay operations supplied the delay by accident. Both helpers now wait `SettlingTimeSec`.

### The connectivity check runs with both PPMUs off the pins

`CheckDgsCapConnectivity` is purely reference -> net -> meter (K3, KN/KF, KO/KH, K1/K2) and uses no
SMU, so both PXIe-4147 channels are shut down before it. Left enabled they sit at 0 V with remote
sense referenced to the cc common, which floats once KM is released, and a channel regulating against
an undefined sense point can offset the net under measurement.

### Each check owns the relays it sets

`SL02_BBAC_CAP.CheckDgsCapCcCommon` used to set `Km` and leave it closed, while the next function,
`CheckDgsCapConnectivity`, released a relay it never set. `Km` therefore stayed closed across the
whole connectivity check, tying CC COMMON1 onto DGSCapx while K3 held that node at the 1.0 V
reference and the PXIe-4147 was still enabled — loading the reference through the SMU's LO network.
Every check now releases exactly what it closed, so relay state cannot leak between them.

### HMOD writes are full-chain overwrites

`HMODControl.HMOD11to13_24to25` builds a fresh 240-bit waveform and bursts HMOD11/12/13/24/25 as
one shift-register chain. **Every omitted argument defaults to 0 and opens those relays.** The
same applies to `HMOD1to4`, `HMOD5to10`, `HMOD14to18`, `HMOD19to23` and `CHMOD1to6`, each of which
is a separate chain that persists across calls to a different chain.

Consequently relays that must be closed together have to go out in one call —
`RelayID("K21, K22")`, never K21 then K22, which would leave only K22 closed. The BBAC steps
accumulate bits into a `RelayState`/`CapRelayState` and issue a single `Apply` that writes every
register; the DIFFMETER steps use direct calls, each self-contained with all needed relays.

## Setup and Cleanup — Board Variants

The program supports two translator board revisions, which generate the MFLEX +5 V DIB user
supplies from different hardware. `Setup_CleanUp.PowerSupply_Setup`, `DCSetup` and
`PowerSupply_CleanUp` each take a `string boardVariant` parameter.

Select the revision in one place with a TestStand file global:

1. **Sequence File Globals** → add `BoardVariant`, type **String**, default value `089357`.
2. On each of the three steps, set the `boardVariant` module parameter's Value expression to
   `FileGlobals.BoardVariant`.

The parameter is a String rather than the `BoardVariant` enum because the TestStand .NET adapter's
handling of enum parameters varies between versions, whereas a String binds to a file global
directly. `Setup_CleanUp.ParseBoardVariant` converts it, matching the drawing number anywhere in the
string, so `089357`, `02-089357-01-a` and `Board089357` are all accepted, case as given. An empty or
unrecognized value throws with the list of valid values instead of defaulting, because silently
guessing would either force the PXIe-4147 onto the BBAC nets of 02-089357 or leave the +5 V DIB
rails dead on 02-084291. `DCSetup` validates the string too, so a typo fails at the first setup step
rather than surfacing later as unexplained measurements.

| | 02-084291-01-c (`BoardVariant.Board084291`) | 02-089357-01-a (`BoardVariant.Board089357`) |
|---|---|---|
| +5V_1/2/3 source | LT1529CQ-5 LDOs U12/U13/U14 | Isolated CCG15-24-05S DC/DC converters M2/M3/M4 |
| LDO / converter input | PXIe-4147 CH0, CH1, CH2 driven to 6.5 V at a 2 A limit | `P102_PS1_AUX_24V_HI`, `P102_PS2_AUX_24V_HI`, `P179_PS2_AUX_12V_HI` |
| Can setup control the rails? | Yes, by forcing the 4147 | No, they follow the AUX supplies; no SMU or relay in the path |
| PXIe-4147 usage | +5 V LDO drivers on CH0-CH2, CH3 unused | BBAC SOURCE CH1/CH2 common mode on CH0/CH1, BBAC CAPTURE CH1 `ACC_CAP1+/-` on CH2/CH3 |
| 4147 `LO_S` | Pin 3 of U12/U13/U14, single shared LO so the rails cannot be stacked | CC COMMON1 / CC COMMON2 star grounds via HMOD4 K25/K26 and HMOD13 |
| Rail read-back | HMOD18 K17/K18/K19 into `P143_4081_DMM` | Identical |

Everything lives in `Hardware/SetupCleanup.cs`, class `Setup_CleanUp`. Only the three TestStand
entry points are public:

| Member | Visibility | Purpose |
|---|---|---|
| `PowerSupply_Setup` | public | ProcessSetup entry point; dispatches on board revision |
| `DCSetup` | public | ALLDC and digital PPMU aperture; one shared body |
| `PowerSupply_CleanUp` | public | ProcessCleanup entry point; dispatches on board revision |
| `BoardVariant` enum | private | The two revisions |
| `Setup084291` / `CleanUp084291` | private | Forces 6.5 V at 2 A onto the `DIB_SMU4147_POS5V` pin group |
| `Setup089357` / `CleanUp089357` | private | Forces nothing; the rails follow the AUX supplies |
| `ParkBbacPpmus` | private | 089357 only: parks 4147 CH0-CH3 at 0 V / 10 mA, output disabled and disconnected |
| `EnableNeg5VSupply` / `DisableNeg5VSupply` | private | `SMU4139_POS5V` -5 V rail, identical on both boards |
| `SetLoadBoardSupplies` | private | Shared `LoadBoardCtrl` enable set |
| `CleanUpUnknownBoard` | private | Board-agnostic teardown for when the revision is unknown |
| `ParseBoardVariant` | private | String to enum, throws on anything unrecognized |
| `TryParseBoardVariant` | private | Non-throwing form, used only by cleanup |

### Setup fails loud, cleanup fails safe

`PowerSupply_Setup` and `DCSetup` throw on an empty or unrecognized `TranslatorBoard`, because
starting a run against the wrong supply topology is worse than not starting.

`PowerSupply_CleanUp` deliberately does **not** throw. Cleanup can be reached before the
`Get Test Settings` step has populated the FileGlobal, or after a failure anywhere earlier in the
run. Throwing there would leave the AUX 24 V / 12 V / 48 V rails and the -5 V supply energized while
masking the original failure, so an unrecognized value falls back to `CleanUpUnknownBoard`, which
parks the PXIe-4147 channels, drops the -5 V rail and opens every load board supply. Nested
try/finally means a fault in one stage still lets the remaining rails come down, and the load board
supplies go last because they feed the rest.

### TestStand wiring

`Get Test Settings` parses the load file configuration into FileGlobals, and must run before
`PowerSupply_Setup` and `DCSetup`. The board-relevant steps are wrapped in flow control:

```
If      FileGlobals.TestInformation.TranslatorBoard == "089357"
ElseIf  FileGlobals.TestInformation.TranslatorBoard == "084291"
Else    <raise error>
End
```

The `If` comparison is exact-match while `TryParseBoardVariant` is substring-match, so keep the
configuration values exactly `084291` and `089357` in all four load files. A decorated value such as
`02-089357-01-a` would set the board up correctly in code while silently skipping the board-relevant
steps in the sequence.

Pinmap change affecting both revisions: the pins on `SMU_4147_C2_S16` channels 0-2 were renamed
from `DIB_POS5V_1/2/3` to `P103_4147_SMU_CH0/CH1/CH2`, matching the schematic net names. The
`DIB_SMU4147_POS5V` pin group still contains those three pins and is used only by the 02-084291
setup. This rename also removed the stale 2.0 A current limit that `SetupCleanup` left on 4147 CH2,
which had been failing the BBAC capture check with `IviCDriverException: Requested Value 2.0 /
Maximum Value 10.0e-3, Property: Current Limit`.

`TBChecker.pinmap` is now the only pinmap. `CHMOD.pinmap` was deleted: every pin, pin group and
instrument name it declared was already present in `TBChecker.pinmap`, and it was missing
`P103_4147_SMU_CH3` and the whole PXIe-4467 (`P122_4467_*`) set, so the BBAC checkers and
`SetupCleanup089357.ParkBbacPpmus` could not have run under it.

`DCSetup` takes the board parameter but currently has one shared body; no 084291-versus-089357
difference has been identified for it yet.

### Auxiliary supply naming

All four fixed-voltage auxiliary channels are sub-channels of the single `AuxPs1` instrument, and
all four are strapped positive on this hardware. `LoadBoardCtrl.EnableLoadBoardSupplies` previously
exposed `AuxPs1/0` as a parameter named `enableNeg24vAtP102`, documented as a "Negative 24V Aux
Supply", which does not exist. It forwarded positionally to `enablePos24vAtP102Ch0`, so behavior was
correct, but a caller who disabled the apparent negative half of a bipolar rail would have silently
killed `+5V_1` on 02-089357. The parameters are now `enablePos24vAtP102Ch0` / `...Ch1` on both
`EnableLoadBoardSupplies` and `EnableFixedVoltageSupplies`. **TestStand steps that bind these
arguments by name need updating.**

| Resource | Block / channel | Schematic net | Load on 02-089357 |
|---|---|---|---|
| `AuxPs1/0` | P102 CH0 (CHy) | `P102_PS1_AUX_24V_HI/LO` | M2 converter → `+5V_1` |
| `AuxPs1/1` | P102 CH1 (CHx) | `P102_PS2_AUX_24V_HI/LO` | M3 converter → `+5V_2` |
| `AuxPs1/2` | P179 CH2 (CHy) | `P179_PS2_AUX_12V_HI/LO` | M4 converter → `+5V_3` |
| `AuxPs1/3` | P179 CH3 (CHx) | `P179_PS1_AUX_48V_HI/LO` | M5 converter → `+12V` |

The schematic's `PS1` / `PS2` prefix is a per-connector position label, not a channel index, and it
does not run the same direction on both connectors: at P102 `PS1` is CHy and `PS2` is CHx, while at
P179 `PS2` is CHy and `PS1` is CHx. Use the table rather than inferring from either naming scheme.
Schematic note 1 of 02-089357 allows either polarity by tying the HI or the LO leg to PGND, and
forbids stacking the aux supplies.

## Build

Open `Code Modules/TBChecker.sln` in Visual Studio. All DLL dependencies are in `bin/`.

## Branch Info

- **main** — Original vXXXXX2a Timeclock project (ADuM225N)
- **mau-branch** — P2Checker project built on mflexa-to-gp3 baseline with improved MFlexMigration and InstrCtrl
