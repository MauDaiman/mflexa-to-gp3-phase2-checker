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
| POOL2_Rise_Time_Check | 10%–90% rise time measurement of 1kΩ+1µF RC via TFE window comparator |
| SL04_DIFFMETER_Check | SL04 differential meter check via HMOD24 + HMOD13 K12/K15 |
| SL10_DIFFMETER_Check | SL10 differential meter check via HMOD24 (chPair 1–5) / HMOD25 (chPair 6–10) + HMOD13 K13/K16 |
| SL24_DIFFMETER_Check | SL24 differential meter check via HMOD25 + HMOD13 K14/K17 |

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

## Build

Open `Code Modules/TBChecker.sln` in Visual Studio. All DLL dependencies are in `bin/`.

## Branch Info

- **main** — Original vXXXXX2a Timeclock project (ADuM225N)
- **mau-branch** — P2Checker project built on mflexa-to-gp3 baseline with improved MFlexMigration and InstrCtrl
