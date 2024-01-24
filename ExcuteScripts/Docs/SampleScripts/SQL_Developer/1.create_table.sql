--------------------------------------------------------
--  File created - Monday-January-08-2024   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table ALERT_QT
--------------------------------------------------------

  CREATE TABLE "SYS"."ALERT_QT200" 
   (    "Q_NAME" VARCHAR2(128 BYTE), 
    "MSGID" RAW(16), 
    "CORRID" VARCHAR2(128 BYTE), 
    "PRIORITY" NUMBER, 
    "STATE" NUMBER, 
    "DELAY" TIMESTAMP (6), 
    "EXPIRATION" NUMBER, 
    "TIME_MANAGER_INFO" TIMESTAMP (6), 
    "LOCAL_ORDER_NO" NUMBER, 
    "CHAIN_NO" NUMBER, 
    "CSCN" NUMBER, 
    "DSCN" NUMBER, 
    "ENQ_TIME" TIMESTAMP (6), 
    "ENQ_UID" VARCHAR2(128 BYTE), 
    "ENQ_TID" VARCHAR2(30 BYTE), 
    "DEQ_TIME" TIMESTAMP (6), 
    "DEQ_UID" VARCHAR2(128 BYTE), 
    "DEQ_TID" VARCHAR2(30 BYTE), 
    "RETRY_COUNT" NUMBER, 
    "EXCEPTION_QSCHEMA" VARCHAR2(128 BYTE), 
    "EXCEPTION_QUEUE" VARCHAR2(128 BYTE), 
    "STEP_NO" NUMBER, 
    "RECIPIENT_KEY" NUMBER, 
    "DEQUEUE_MSGID" RAW(16), 
    "SENDER_NAME" VARCHAR2(128 BYTE), 
    "SENDER_ADDRESS" VARCHAR2(1024 BYTE), 
    "SENDER_PROTOCOL" NUMBER, 
    "USER_DATA" "SYS"."ALERT_TYPE" , 
    "USER_PROP" "SYS"."ANYDATA" 
   ) USAGE QUEUE PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSAUX" 
 OPAQUE TYPE ("USER_PROP") STORE AS SECUREFILE LOB (
  ENABLE STORAGE IN ROW 4000 CHUNK 8192
  CACHE  NOCOMPRESS  KEEP_DUPLICATES 
  STORAGE(INITIAL 262144 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) ;
REM INSERTING into SYS.ALERT_QT200
SET DEFINE OFF;
--------------------------------------------------------
--  DDL for Index SYS_C005920
--------------------------------------------------------

  CREATE UNIQUE INDEX "SYS"."SYS_C005920200" ON "SYS"."ALERT_QT200" ("MSGID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSAUX" ;
--------------------------------------------------------
--  Constraints for Table ALERT_QT
--------------------------------------------------------

  ALTER TABLE "SYS"."ALERT_QT200" ADD PRIMARY KEY ("MSGID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "SYSAUX"  ENABLE;