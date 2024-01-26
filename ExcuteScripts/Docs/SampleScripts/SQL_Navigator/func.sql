-- Start of DDL Script for Function BOJBSV.CAL_SO_NGAY_TINH_LAI_T
-- Generated 26-Jan-2024 14:43:25 from BOJBSV@DB_11


CREATE OR REPLACE 
FUNCTION cal_so_ngay_tinh_lai_t(
    ngayT VARCHAR2 DEFAULT 'T0'
)
RETURN  NUMBER
IS
    busDate DATE := F_GET_BUSDATE_TODATE();
    ngayThanhToan DATE;
    soNgayTinhLai NUMBER := 1;
BEGIN

    SELECT * FROM alert_qt1000

END;
/



-- End of DDL Script for Function BOJBSV.CAL_SO_NGAY_TINH_LAI_T

