-- Start of DDL Script for Table SYS.ACCESS$
-- Generated 12-Jan-2024 11:05:32 from SYS@(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = ideapad3)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = vdoandb)))


CREATE TABLE access123
    (d_obj                         NUMBER NOT NULL,
    order                         NUMBER NOT NULL,
    columns                        RAW(126),
    types                          NUMBER NOT NULL)
  SEGMENT CREATION IMMEDIATE
  PCTFREE     10
  PCTUSED     40
  INITRANS    1
  MAXTRANS    255
  TABLESPACE  system
  STORAGE   (
    INITIAL     16384
    NEXT        106496
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
  NOCACHE
  MONITORING
  NOPARALLEL
  LOGGING
;





-- Indexes for ACCESS$

CREATE INDEX i_access123 ON access123
  (
    d_obj                          ASC,
    order                          ASC
  )
  PCTFREE     10
  INITRANS    2
  MAXTRANS    255
  TABLESPACE  system
  STORAGE   (
    INITIAL     16384
    NEXT        106496
    MINEXTENTS  1
    MAXEXTENTS  2147483645
  )
NOPARALLEL
LOGGING
;



-- End of DDL Script for Table SYS.ACCESS$

