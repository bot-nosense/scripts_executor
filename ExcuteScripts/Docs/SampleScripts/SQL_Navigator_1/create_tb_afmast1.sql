-- Start of DDL Script for Table BOJBSV.AFMAST
-- Generated 2/2/2024 1:44:16 PM from BOJBSV@DB_11


CREATE TABLE afmast
    (actype                        VARCHAR2(4 BYTE) NOT NULL,
    custid                         VARCHAR2(20 BYTE),
    acctno                         VARCHAR2(20 BYTE) NOT NULL,
    aftype                         VARCHAR2(3 BYTE),
    tradefloor                     VARCHAR2(1 BYTE),
    tradetelephone                 VARCHAR2(1 BYTE),
    tradeonline                    VARCHAR2(1 BYTE),
    pin                            VARCHAR2(500 BYTE),
    language                       VARCHAR2(3 BYTE) NOT NULL,
    tradephone                     VARCHAR2(100 BYTE),
    allowdebit                     VARCHAR2(1 BYTE),
    bankacctno                     VARCHAR2(20 BYTE),
    bankname                       VARCHAR2(3 BYTE),
    swiftcode                      VARCHAR2(20 BYTE),
    receivevia                     VARCHAR2(1 BYTE),
    email                          VARCHAR2(100 BYTE),
    address                        VARCHAR2(4000 BYTE),
    fax                            VARCHAR2(100 BYTE),
    ciacctno                       VARCHAR2(20 BYTE),
    ifrulecd                       VARCHAR2(3 BYTE),
    lastdate                       DATE,
    status                         VARCHAR2(1 BYTE),
    pstatus                        VARCHAR2(50 BYTE),
    marginline                     NUMBER,
    tradeline                      NUMBER,
    advanceline                    NUMBER,
    repoline                       NUMBER,
    depositline                    NUMBER,
    bratio                         NUMBER DEFAULT 100,
    ucbratio                       NUMBER DEFAULT 100,
    termofuse                      VARCHAR2(3 BYTE),
    description                    VARCHAR2(4000 BYTE),
    iccfcd                         CHAR(5 BYTE),
    iccftied                       CHAR(1 BYTE) DEFAULT 'Y',
    telelimit                      NUMBER DEFAULT 0,
    onlinelimit                    NUMBER DEFAULT 0,
    cftelelimit                    NUMBER DEFAULT 0,
    cfonlinelimit                  NUMBER DEFAULT 0,
    traderate                      NUMBER DEFAULT 0,
    deporate                       NUMBER DEFAULT 0,
    miscrate                       NUMBER DEFAULT 0,
    fax1                           VARCHAR2(100 BYTE),
    phone1                         VARCHAR2(100 BYTE),
    stmcycle                       VARCHAR2(3 BYTE),
    isotc                          CHAR(1 BYTE) DEFAULT 'N',
    consultant                     VARCHAR2(1 BYTE),
    pisotc                         VARCHAR2(20 BYTE) DEFAULT 'N',
    opndate                        DATE,
    feetype                        VARCHAR2(4 BYTE),
    ucfeetype                      VARCHAR2(4 BYTE),
    tlid                           VARCHAR2(4 BYTE),
    firmcd                         VARCHAR2(4 BYTE),
    brokerid                       VARCHAR2(160 BYTE),
    introid                        VARCHAR2(160 BYTE),
    loanused                       VARCHAR2(1 BYTE) DEFAULT 'N',
    lntype                         VARCHAR2(4 BYTE),
    brtype                         VARCHAR2(10 BYTE),
    manty                          VARCHAR2(4 BYTE) DEFAULT 'NM',
    autopaymg                      VARCHAR2(100 BYTE) DEFAULT 'Y',
    tempcrline                     NUMBER DEFAULT 0,
    cltype                         VARCHAR2(4 BYTE),
    cusbankname                    VARCHAR2(500 BYTE),
    branchbank                     VARCHAR2(500 BYTE),
    advtype                        VARCHAR2(4 BYTE),
    tradingbylist                  VARCHAR2(1 BYTE),
    creditlimit                    NUMBER DEFAULT 0,
    payintamt                      VARCHAR2(1 BYTE) DEFAULT 'Y',
    dislimit                       VARCHAR2(1 BYTE) DEFAULT 'N',
    commiss                        VARCHAR2(1 CHAR) DEFAULT 'N',
    created_by                     VARCHAR2(50 BYTE),
    created_date                   DATE,
    approved_by                    VARCHAR2(50 BYTE),
    approved_date                  DATE,
    broker_type                    NUMBER,
    customer_id                    VARCHAR2(255 CHAR),
    bidvonline                     VARCHAR2(1 BYTE),
    bankconnect                    VARCHAR2(20 BYTE) DEFAULT 'NONE',
    bankaccount                    VARCHAR2(50 BYTE))
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
;





-- Comments for AFMAST

COMMENT ON COLUMN afmast.commiss IS 'TK UY THAC HAY KHONG'
;
COMMENT ON COLUMN afmast.creditlimit IS 'HM bao lanh cap them'
;
COMMENT ON COLUMN afmast.dislimit IS 'N:KHong khau tru,Y: CO KHAU TRU'
;
COMMENT ON COLUMN afmast.manty IS 'MAC DINH LA NM'
;
COMMENT ON COLUMN afmast.payintamt IS 'co tinh lai hay k'
;

-- End of DDL Script for Table BOJBSV.AFMAST

