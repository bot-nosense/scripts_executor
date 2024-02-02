-- Start of DDL Script for Table BOJBSV.CLTYPE
-- Generated 2/2/2024 11:03:23 AM from BOJBSV@DB_11


CREATE TABLE cltype_2
    (actype                         VARCHAR2(4 BYTE) NOT NULL,
    typename                       VARCHAR2(1000 BYTE),
    mglimit                        NUMBER DEFAULT 0,
    mgrate                         NUMBER DEFAULT 0,
    mgrateinit                     NUMBER DEFAULT 0,
    mgcall                         NUMBER DEFAULT 0,
    mgsell                         NUMBER DEFAULT 0,
    mgbuy                          NUMBER DEFAULT 0,
    mgcalltype                     VARCHAR2(1 BYTE) DEFAULT 'N',
    mgexetype                      VARCHAR2(1 BYTE) DEFAULT 'N',
    ruletype                       VARCHAR2(1 BYTE) DEFAULT 'N',
    inttype                        VARCHAR2(1 BYTE) DEFAULT 'D',
    iccftype                       VARCHAR2(1 BYTE) DEFAULT 'N',
    frate                          NUMBER DEFAULT 0,
    prate                          NUMBER DEFAULT 0,
    minval                         NUMBER DEFAULT 0,
    maxval                         NUMBER DEFAULT 0,
    maxintrate                     NUMBER DEFAULT 0,
    intday                         VARCHAR2(3 BYTE),
    termlnsts                      VARCHAR2(1 BYTE) DEFAULT 'N',
    termlntype                     VARCHAR2(1 BYTE) DEFAULT 'N',
    termloan                       NUMBER DEFAULT 0,
    acrloan                        NUMBER DEFAULT 0,
    monthday                       VARCHAR2(1 BYTE) DEFAULT 'N',
    yearday                        VARCHAR2(1 BYTE) DEFAULT 'N',
    status                         VARCHAR2(1 BYTE) DEFAULT 'N',
    lastdate                       DATE,
    tlid                           VARCHAR2(4 BYTE),
    description                    VARCHAR2(1000 BYTE),
    acrloanexp                     NUMBER DEFAULT 0,
    intrateexp                     NUMBER DEFAULT 0,
    mgtype                         NUMBER DEFAULT 0,
    intbankday                     VARCHAR2(3 BYTE) DEFAULT '000')
  PCTFREE     10
  INITRANS    1
  MAXTRANS    255
  TABLESPACE  users
  NOCACHE
  MONITORING
  NOPARALLEL
  LOGGING
/





-- End of DDL Script for Table BOJBSV.CLTYPE

