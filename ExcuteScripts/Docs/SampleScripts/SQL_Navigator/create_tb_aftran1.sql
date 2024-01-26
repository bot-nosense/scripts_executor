-- Start of DDL Script for Table BOJBSV.AFTRAN
-- Generated 24-Jan-2024 15:53:32 from BOJBSV@DB_11


CREATE TABLE aftran1000
    (txnum                          NVARCHAR2(30),
    txdate                         DATE,
    acctno                         VARCHAR2(20 BYTE),
    txcd                           VARCHAR2(4 BYTE),
    namt                           NUMBER,
    camt                           VARCHAR2(50 BYTE),
    ref                            VARCHAR2(20 BYTE),
    deltd                          VARCHAR2(1 BYTE),
    tltxcd                         VARCHAR2(10 BYTE),
    field                          VARCHAR2(50 BYTE),
    tablename                      VARCHAR2(50 BYTE),
    txtype                         VARCHAR2(10 BYTE),
    busdate                        DATE)
  SEGMENT CREATION IMMEDIATE
  PCTFREE     10
  INITRANS    1
  MAXTRANS    255
  TABLESPACE  users
  STORAGE   (
    INITIAL     65536
    NEXT        1048576
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
  NOCACHE
  MONITORING
  NOPARALLEL
  LOGGING
/





-- End of DDL Script for Table BOJBSV.AFTRAN

