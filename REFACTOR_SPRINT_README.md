# POSDeployTool – Architecture Refactor Sprint

## เป้าหมาย

แยก Business Logic ออกจาก `frmMain.vb` โดยไม่แก้ `frmMain.Designer.vb` และไม่เปลี่ยนพฤติกรรมเดิมของหน้าจอ

## โครงสร้างใหม่

```text
POSDeployTool
├─ Application
│  ├─ ConnectionCheckController.vb
│  └─ ConnectionCheckProgressEventArgs.vb
├─ Contracts
│  ├─ IPingService.vb
│  ├─ IStoreConfigService.vb
│  └─ IWinRmService.vb
├─ Presentation
│  └─ ConnectionStatusPresenter.vb
├─ Models
├─ Services
├─ Helpers
├─ frmMain.vb
└─ frmMain.Designer.vb
```

## หน้าที่ของแต่ละ Layer

- `frmMain`: UI events, binding, grid และการแสดง MessageBox เท่านั้น
- `ConnectionCheckController`: orchestration ของ Ping/WinRM, parallelism, cancellation และอัปเดต `ConnectionState`
- `ConnectionStatusPresenter`: แปลง state เป็นข้อความและสีใน Grid
- `Contracts`: interface ของ service เพื่อรองรับ test/mocking และเปลี่ยน implementation ภายหลัง
- `Services`: งาน infrastructure จริง เช่น Ping, WinRM และอ่าน stores.json
- `Models`: data/state เท่านั้น

## สิ่งที่คงเดิม

- `frmMain.Designer.vb`
- ชื่อ Controls และ Event เดิม
- stores.json
- การ Ping และตรวจ WinRM
- Deploy button เปิดเมื่อ Store มี `CanDeploy = True`
- Cancellation และ Logging

## วิธีใช้งาน

1. เปิด `POSDeployTool.sln`
2. Restore NuGet package หาก Visual Studio แจ้งเตือน
3. เลือก `Build > Rebuild Solution`
4. ทดสอบ Reload, Filter, Check Connection และ Stop

## Git commit ที่แนะนำ

```text
Refactor connection workflow into application layers
```

## Sprint ถัดไป

เพิ่ม Deployment Engine โดยสร้าง Controller/Service แยกจาก UI เช่น:

```text
Application/DeploymentController.vb
Contracts/IDeploymentService.vb
Models/DeploymentState.vb
Services/DeploymentService.vb
```
