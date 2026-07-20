# POS Deploy Tool — Sprint 3.5

ฟังก์ชัน:
- Ping / WinRM check
- Backup `C:\BJCBCPOS` เป็น `C:\BJCBCPOS_yyyyMMdd`
- Stop process
- Deploy ไฟล์จาก `Release\BJCBCPOS` ผ่าน SMB administrative share
- Robocopy และตรวจจำนวนไฟล์
- Start/verify process (ตัวเลือก)

ก่อน Deploy ให้วางไฟล์ release จริงใน `POSDeployTool\Release\BJCBCPOS` และลบไฟล์ `PUT_RELEASE_FILES_HERE.txt` ออก

หมายเหตุ: เครื่องที่รัน Tool ต้องเข้าถึง `\IP\C$` ได้ด้วย Username/Password ใน `stores.json`
