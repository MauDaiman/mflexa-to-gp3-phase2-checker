# P2Checker — Phase 2 Translator Board Self-Test

NI STS C# test program for verifying GP3 translator board health prior to DUT testing.

## Project Structure

```
├── Code Modules/
│   ├── InstrCtrl/          — Instrument control wrappers (DCPower, Digital, DMM, DAQmx, etc.)
│   ├── MFlexMigration/     — MFlex-to-GP3 migration framework (HMOD, relay, pattern exec)
│   ├── TestSteps/
│   │   ├── Common/         — Shared helpers (MeterInstance, HMODCtrl)
│   │   └── Modules/        — Individual board check test steps
│   └── P2Checker.sln       — Visual Studio solution
├── Limits/                  — Test limit files
├── STDF File/               — STDF result files
├── Supporting Materials/
│   ├── Digital/             — Compiled patterns, waveforms, levels, timing
│   ├── Pin Maps/            — P2CPinmap.pinmap
│   ├── Calibration Data/
│   └── Offline Configuration/
├── bin/                     — Pre-built DLLs and NI driver assemblies
└── STSCsharp_P2Checker.seq  — TestStand sequence file
```

## Test Modules

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

## Build

Open `Code Modules/P2Checker.sln` in Visual Studio. All DLL dependencies are in `bin/`.

## Branch Info

- **main** — Original vXXXXX2a Timeclock project (ADuM225N)
- **mau-branch** — P2Checker project built on mflexa-to-gp3 baseline with improved MFlexMigration and InstrCtrl
