\c postgres;
DROP DATABASE hotel_db;
CREATE DATABASE hotel_db;
\c hotel_db;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION postgis;
-----
-----


-----
-----
CREATE TABLE images(
    imageid UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(200) NOT NULL,
    belongTo UUID NOT NULL,
    isThumnail BOOLEAN DEFAULT FALSE
);
-----
-----



-----
-----
CREATE TABLE Persons (
    PersonID UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Phone VARCHAR(13) NOT NULL UNIQUE,
    Address TEXT NULL
);
-----
-----



-----
-----
CREATE TABLE Users (
    UserID UUID PRIMARY KEY,
    UserName VARCHAR(50) NOT NULL UNIQUE,
    Password TEXT NOT NULL,
    DateOfBirth DATE NOT NULL,
    IsVIP bool DEFAULT FALSE,
    PersonID UUID NOT NULL REFERENCES Persons (personid),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP, 
    UpdateAt TIMESTAMP DEFAULT NULL,
    IsDeleted bool DEFAULT FALSE,
    addBy UUID DEFAULT NULL,
    Role Int DEFAULT 0  check(Role=0 OR Role=1)
);
----- role 0 is user 1 is admin
-----



-----
-----


-----
-----
CREATE OR REPLACE FUNCTION isAdmin(id UUID)
RETURNS BOOLEAN AS $$
DECLARE is_exist_id BOOLEAN;
BEGIN

SELECT COUNT(*) >= 1 INTO is_exist_id
FROM persons per
    LEFT JOIN users usr ON per.personid = usr.personid
WHERE usr.role =1 ;

RETURN is_exist_id;
EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN FALSE;
END;
$$LANGUAGE plpgsql;
-----
-----



-----
-----
CREATE VIEW usersview AS
SELECT per.PersonID,
    per.Name,
    per.Phone,
    per.Email,
    per.Address,
    use.UserID,
    use.DateOfBirth,
    use.UserName,
    use.IsVIP,
    use.CreatedAt,
    use.password,
    use.IsDeleted as ispersondeleted,
    use.role
FROM users use
    INNER JOIN persons per ON use.personid = per.personid
    ORDER BY use.CreatedAt DESC
    ;
-----
-----



-----
-----
CREATE OR REPLACE FUNCTION getUserPagination(pageNumber INT, limitNumber INT)
RETURNS TABLE (
    PersonId UUID,
    Name VARCHAR(50),
    Phone VARCHAR(13),
    Email VARCHAR(100),
    Address TEXT,
    UserId UUID,
    IsDeleted BOOL,
    UserName VARCHAR(50),
    DateOfBirth DATE,
    IsVIP BOOL,
    CreatedAt TIMESTAMP
) AS $$ BEGIN RETURN QUERY
SELECT per.PersonId,
    per.Name,
    per.Phone,
    per.Email,
    per.Address,
    us.UserId,
    us.IsDeleted,
    us.UserName,
    us.DateOfBirth,
    us.IsVIP,
    us.CreatedAt
FROM Persons per
    INNER JOIN Users us ON per.PersonID = us.PersonID
ORDER BY us.CreatedAt DESC
LIMIT limitNumber OFFSET limitNumber * (pageNumber - 1);
EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN QUERY
SELECT Null,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL;
END $$ LANGUAGE plpgsql;
-----
-----



-----
-----

CREATE OR REPLACE FUNCTION fn_user_insert_in (
userId_u UUID,
name_ VARCHAR(50),
phone_ VARCHAR(13),
email_ VARCHAR(100),
address_ TEXT,
username_ varchar(50),
password_ TEXT,
DateOfBirth_ DATE,
addby_ UUID,
role_ INT
) RETURNS INT AS $$
DECLARE 
person_id UUID;
is_there_already_admin BOOLEAN;
is_already_user_name_And_id BOOLEAN;
BEGIN

IF role_ = 1 THEN
        SELECT COUNT(*)>0  INTO is_there_already_admin FROM users WHERE role =1;
        IF is_there_already_admin = TRUE THEN 
          RAISE EXCEPTION 'Something went wrong: only one admin can be set';
          RETURN 0;
        END IF ;
END IF;

---validation for is already there username and id in database 
SELECT COUNT(*)>0 INTO is_already_user_name_And_id 
FROM users WHERE username = username_ OR UserID=userId_u;

IF is_already_user_name_And_id=TRUE THEN 
    RAISE EXCEPTION 'Something went wrong: username or userid is already exist';
    RETURN 0;
END IF;

---validate user age

IF DateOfBirth_ > CURRENT_DATE - INTERVAL '18 years' THEN 
    RAISE EXCEPTION 'Something went wrong: account must be creation for adult';
    RETURN 0;
END IF;



INSERT INTO persons(name, email, phone, address)
VALUES (name_, email_, phone_, address_)
RETURNING personid INTO person_id;



INSERT INTO Users (
        userid,
        dateofbirth,
        username,
        password,
        personid,
        addby,
        role
    )
VALUES(
        userId_u,
        DateOfBirth_,
        username_,
        password_,
        person_id,
        addby_,
        role_
    );




RETURN 1;
EXCEPTION
WHEN OTHERS THEN 


RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;

RETURN 0;
END;
$$ LANGUAGE plpgsql;
-----
-----


-----
CREATE OR REPLACE FUNCTION fn_user_update(
userid_u UUID,
name_u VARCHAR(50),
phone_u VARCHAR(13),
address_u TEXT,
username_u VARCHAR(50),
password_u TEXT,
IsVIP_u BOOLEAN,
personid_u UUID,
updatedBy UUID
) RETURNS INT AS $$
DECLARE
is_exist_userName BOOLEAN;
is_hav_validation_toSet_to_vip BOOLEAN;
is_admin BOOLEAN;
BEGIN

	is_admin:= isAdmin(updatedBy);
	IF is_admin = false THEN
		RAISE EXCEPTION 'only admin can update data';
RETURN 0;
END IF ; 



UPDATE persons
SET name = CASE
               WHEN name <> name_u AND name_u IS NOT NULL  THEN name_u
               ELSE name
    END,
    phone = CASE
                WHEN phone <> phone_u AND phone_u IS NOT NULL THEN phone_u
                ELSE phone
        END,
    address = CASE
                  WHEN address <> address_u  AND address_u IS NOT NULL  THEN address_u
                  ELSE address
        END
WHERE personid = personid_u;
---
----
UPDATE users
SET username = CASE
                   WHEN username_u <> username AND username_u  IS NOT NULL THEN username_u
                   ELSE username
    END,
    password = CASE
                   WHEN password_u <> password AND password_u IS NOT NULL THEN password_u
                   ELSE password
        END,
    isvip = CASE
                WHEN IsVIP_u <> isvip AND IsVIP_u IS NOT NULL  THEN IsVIP_u
                ELSE isvip
        END,
    UpdateAt = CURRENT_TIMESTAMP
WHERE userid = userid_u;




RETURN 1;
EXCEPTION
WHEN OTHERS THEN 



RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN NULL;
END;
$$ LANGUAGE plpgsql;
-----
-----


-----
CREATE OR REPLACE FUNCTION fn_delete_user(
userid_ UUID,
deletedBy_ UUID
) RETURNS INT AS $$
DECLARE
is_admin BOOLEAN;
BEGIN

	is_admin:= isAdmin(deletedBy_);
	IF is_admin = false THEN
		RAISE EXCEPTION 'only admin can update data';
        RETURN 0;
    END IF ; 


----
UPDATE users
SET isdeleted = CASE WHEN  isdeleted = TRUE THEN FALSE ELSE TRUE END,
    UpdateAt = CURRENT_TIMESTAMP
WHERE userid = userid_;



RETURN 1;
EXCEPTION
WHEN OTHERS THEN 




RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN NULL;
END;
$$ LANGUAGE plpgsql;
-----
-----


-----
-----
CREATE OR REPLACE FUNCTION isExistById(id UUID)
RETURNS BOOLEAN AS $$
DECLARE isExist BOOLEAN;

BEGIN
SELECT EXISTS(
        SELECT 1 FROM  users WHERE  userid = id AND isdeleted  =  false

    ) INTO isExist;
RETURN isExist;
 
EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN FALSE;
END;
$$LANGUAGE plpgsql;
-----
-----


-----
-----
CREATE OR REPLACE FUNCTION isExistByIdAndEmail(email_hold VARCHAR(100),id UUID)
RETURNS BOOLEAN AS $$
DECLARE isExist BOOLEAN;
BEGIN
    SELECT COUNT(*)>0 INTO isExist
    FROM persons per
    LEFT JOIN users use ON per.personid = use.personid
    WHERE per.email = email_hold
    AND use.userid = id;
    RETURN isExist ;
EXCEPTION
    WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
    SQLERRM;
    RETURN FALSE;
END;
$$LANGUAGE plpgsql;
-----
-----

----
----

CREATE OR REPLACE FUNCTION isExistByEmailAndPhone(email_ TEXT,phone_ VARCHAR)
RETURNS BOOLEAN AS $$
DECLARE isExist BOOLEAN;
BEGIN
    SELECT COUNT(*)>0 INTO isExist 
    FROM persons per
    WHERE email = email_ OR phone =phone_ ;
    RETURN isExist;
EXCEPTION
    WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
    SQLERRM;
    RETURN FALSE;
END;
$$LANGUAGE plpgsql;
----
----

-----
-----
CREATE TABLE RoomTypes (
    RoomTypeID UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name VARCHAR(50) NOT NULL,
    CreatedBy UUID NOT NULL  REFERENCES Users (userid),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT NULL,
    IsDeleted bool DEFAULT FALSE
);
-----
-----


-----
-----
CREATE OR REPLACE FUNCTION fn_roomtype_insert_new(
        roomtype_id_holder UUID,
        name_s VARCHAR,
        createdby_s UUID
    ) RETURNS BOOLEAN AS $$
DECLARE
     roomType_id UUID;
     is_admin BOOLEAN := FALSE;
BEGIN

        is_admin := isAdmin(createdby_s);
        IF is_admin = FALSE THEN
            RAISE EXCEPTION 'only admin can create roomtype';
            RETURN FALSE;
        END IF;


        INSERT INTO RoomTypes(roomtypeid, name, createdby)
        VALUES (roomtype_id_holder, name_s, createdby_s)
        RETURNING RoomTypeID INTO roomType_id;


        

        IF roomType_id IS NOT NULL THEN
             RETURN TRUE;
        ELSE 
            RAISE EXCEPTION 'roomtype not created';
            RETURN FALSE;
        END IF;
EXCEPTION
WHEN OTHERS THEN 

    RAISE EXCEPTION 'You do not have permission to create room type: %',
    SQLERRM;
    RETURN FALSE;
END;
$$ LANGUAGE plpgsql;
-----
-----


-----
-----
CREATE OR REPLACE FUNCTION fn_roomtype_update_new(
        name_s VARCHAR,
        roomtypeid_s UUID,
        createdby_s UUID
    ) RETURNS BOOLEAN AS $$
DECLARE 
is_hasPermission BOOLEAN := FALSE;

BEGIN 
    is_hasPermission := isAdmin(createdby_s);
    if is_hasPermission = FALSE THEN 
        RAISE EXCEPTION 'you do not have permission to update roomtype';
        RETURN FALSE;
    END IF;


    UPDATE RoomTypes
        SET name = CASE
                WHEN name <> name_s AND name_s IS NOT NULL THEN name_s
                ELSE name
            END,
            UpdatedAt =  CURRENT_TIMESTAMP
    WHERE roomtypeid = roomtypeid_s;
    

    RETURN TRUE;

EXCEPTION
WHEN OTHERS THEN 

    RAISE EXCEPTION 'Something went wrong: %', SQLERRM;
    RETURN FALSE;
END;
$$ LANGUAGE plpgsql;
-----
-----





-----
-----
CREATE OR REPLACE FUNCTION fn_roomtype_delete(
deltedBy_id UUID,
roomtype_id UUID
) RETURNS Boolean AS $$ 
DECLARE
is_admin BOOLEAN;

BEGIN

    is_admin := isAdmin(deltedBy_id);
    IF is_admin = FALSE THEN
            RAISE EXCEPTION 'only admin can create roomtype';
            RETURN FALSE;
    END IF;


    UPDATE roomtypes
    SET IsDeleted = CASE
            WHEN isdeleted = TRUE THEN FALSE
            ELSE TRUE
        END
    WHERE roomtypeid = roomtype_id;

    RETURN TRUE;
EXCEPTION
WHEN OTHERS THEN 

    RAISE EXCEPTION 'you do not have permission to delete roomtype';
    RETURN FALSE;
END $$ LANGUAGE plpgsql;
-----
-----



-----
-----

-----
-----

----
----



----
----

CREATE TABLE Rooms (
    RoomID UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Status VARCHAR(10) CHECK (
        Status IN ('Available', 'Booked', 'Under Maintenance')
    ) DEFAULT 'Available',
    pricePerNight NUMERIC(10, 2),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    roomtypeid UUID NOT NULL REFERENCES RoomTypes (roomtypeid),
    capacity INT NOT NULL,
    bedNumber INT NOT NULL,
    belongTo UUID REFERENCES users(userid),
    updateAt TIMESTAMP NULL,
    isBlock BOOLEAN DEFAULT FALSE,
    isDeleted BOOLEAN DEFAULT FALSE,
    location GEOMETRY(Point, 4326)  DEFAULT NULL,
    place text DEFAULT NULL
);
----
----




----
----

CREATE OR REPLACE FUNCTION is_room_belong_to_user(
    room_id UUID,
    user_id UUID
) RETURNS BOOLEAN
AS $$
DECLARE
isBelongTo BOOLEAN;
BEGIN

    SELECT 
        COUNT(*)>0 INTO isBelongTo 
        FROM rooms 
    WHERE roomid = room_id AND belongto = user_id;
    RETURN isBelongTo;
EXCEPTION
    WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
    SQLERRM;
RETURN FALSE;

END;
$$LANGUAGE PLPGSQL;
----
----




----
----
CREATE OR REPLACE FUNCTION getRoomsByPage(
pageNumber INT,
limitNumber INT,
belongId UUID) RETURNS TABLE(
        RoomID UUID,
        Status VARCHAR(10),
        pricePerNight NUMERIC(10, 2),
        CreatedAt TIMESTAMP,
        roomtypeid UUID,
        capacity INT,
        bedNumber INT,
        belongTo UUID,
        isblock Boolean,
        isDeleted Boolean,
        place TEXT,
         longitude NUMERIC,
        latitude NUMERIC
    ) AS $$ BEGIN

    IF pageNumber<1 THEN
    RAISE EXCEPTION 'the pageNumber is not valide ';
    END IF;

RETURN QUERY SELECT
rom.roomid,
    rom.Status,
    rom.pricepernight,
    rom.CreatedAt,
    rom.roomtypeid,
    rom.capacity,
    rom.bedNumber,
    rom.belongTo
    ,rom.isblock,
    rom.isdeleted,
    rom.place,
    ST_X(rom.location)::NUMERIC as longitude,
    ST_Y(rom.location)::NUMERIC as latitude
FROM rooms rom
    INNER JOIN roomtypes romt ON rom.roomtypeid = romt.roomtypeid
WHERE rom.isBlock = FALSE AND (belongId IS  NULL OR rom.belongTo=belongId)
ORDER BY rom.CreatedAt DESC
LIMIT limitNumber OFFSET limitNumber * (pageNumber - 1);
EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN QUERY
SELECT 
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL;
END;
$$ LANGUAGE plpgsql;

----
----CREATE OR REPLACE FUNCTION getRoomsByID(
roomId_ UUID,
userId_ UUID
) RETURNS TABLE(
        RoomID UUID,
        Status VARCHAR(10),
        pricePerNight NUMERIC(10, 2),
        CreatedAt TIMESTAMP,
        roomtypeid UUID,
        capacity INT,
        bedNumber INT,
        belongTo UUID,
        isblock BOOLEAN,
        isDeleted BOOLEAN,
		 place TEXT,
         longitude NUMERIC,
        latitude NUMERIC
    ) AS $$ 
	
DECLARE
isAdmin BOOLEAN:=FALSE;
BEGIN
	IF userId_ IS NOT NULL THEN 
		SELECT role = 1 into isAdmin FROM users WHERE userid = userId_;
	END IF ;
RETURN QUERY SELECT
rom.roomid,
    rom.Status,
    rom.pricepernight,
    rom.CreatedAt,
    rom.roomtypeid,
    rom.capacity,
    rom.bedNumber,
    rom.belongTo
    ,rom.isblock,
    rom.isdeleted,
	rom.place,
    ST_X(rom.location)::NUMERIC as longitude,
    ST_Y(rom.location)::NUMERIC as latitude
FROM rooms rom

WHERE   rom.roomid=roomId_ AND 
(((isAdmin IS  NULL OR isAdmin=FALSE) AND rom.isblock =FALSE)OR (isAdmin=TRUE));

EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN QUERY
SELECT 
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
	NULL
    NULL;
END;
$$ LANGUAGE plpgsql;
----

----
----
CREATE OR REPLACE FUNCTION fn_room_insert_new(
         room_id UUID,
        status VARCHAR(10),
        pricePerNight_ NUMERIC(10, 2),
        roomtypeid_ UUID,
        capacity_ INT,
        bedNumber_ INT,
        belongTo_ UUID,
        location_ GEOMETRY ,
        place_ text 
    ) RETURNS UUID AS $$
DECLARE roomid_holder UUID;
is_hasPermission_to_delete BOOLEAN;
BEGIN
INSERT INTO rooms(
        roomid,
        Status,
        pricePerNight,
        roomtypeid,
        capacity,
        bedNumber,
        belongTo,
        location,
        place
    )
VALUES (
        room_id,
        status,
        pricePerNight_,
        roomtypeid_,
        capacity_,
        bedNumber_,
        belongTo_,
        location_,
        place_
    )
RETURNING roomid INTO roomid_holder;
 
RETURN roomid_holder;
EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN NULL;
END;
$$ LANGUAGE PLPGSQL;
---
---

---
---
-- CREATE OR REPLACE FUNCTION fn_room_insert() RETURNS TRIGGER AS $$
-- DECLARE isUserDeleted BOOLEAN;
-- BEGIN
--     SELECT isdeleted INTO isUserDeleted
--     FROM users
--     WHERE userid = NEW.userid;
    
--     IF isUserDeleted IS NOT NULL
--          AND isUserDeleted = TRUE THEN RETURN NEW;
--     END IF;

-- RETURN NULL;
-- EXCEPTION
--     WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
--     SQLERRM;
--     RETURN NULL;
-- END;
-- $$ LANGUAGE plpgsql;
---
---

---
----

CREATE OR REPLACE FUNCTION fn_room_update_new(
        roomid_ UUID,
        status_ VARCHAR(10),
        pricePerNight_ NUMERIC(10, 2),
        roomtypeid_ UUID,
        capacity_ INT,
        bedNumber_ INT,
        belongTo_ UUID,
        location_ GEOMETRY ,
        place_ text 
    ) RETURNS Boolean AS $$
DECLARE updateAt_holder TIMESTAMP;
BEGIN
    IF EXISTS(
	 SELECT 1 FROM rooms WHERE roomid=roomid_
	 AND belongTo <> belongTo_
	) THEN
     RAISE EXCEPTION 'only room owner can update room data';
	END IF ;

    UPDATE rooms
    SET Status = CASE
            WHEN status <> status_
            AND status_ IS NOT NULL THEN status_
            ELSE status
        END,
        pricePerNight = CASE
            WHEN pricePerNight_ <>  pricePerNight
            AND pricePerNight_ IS NOT NULL THEN pricePerNight_
            ELSE  pricePerNight
        END,
        roomtypeid = CASE
            WHEN roomtypeid_ <>  roomtypeid
            AND roomtypeid_ IS NOT NULL THEN roomtypeid_
            ELSE  roomtypeid
        END,
        capacity = CASE
            WHEN capacity_ <>  capacity
            AND capacity_ IS NOT NULL THEN capacity_
            ELSE  capacity
        END,
        bedNumber = CASE
            WHEN bedNumber_ <>  bedNumber
            AND bedNumber_ IS NOT NULL THEN bedNumber_
            ELSE  bedNumber
        END,
		location= CASE
            WHEN location_ <>  location
            AND location_ IS NOT NULL THEN location_
            ELSE  location
        END,
		place=CASE
            WHEN place_ <>  place
            AND place_ IS NOT NULL THEN place_
            ELSE  place
        END
		,
        updateAt = CURRENT_TIMESTAMP
    WHERE roomid = roomid_;
    SELECT updateAt INTO updateAt_holder
    FROM rooms
    WHERE roomid = roomid_;

    RETURN updateAt_holder is NOT NULL;

EXCEPTION

    WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
    SQLERRM;
    RETURN false;
END;
$$ LANGUAGE PLPGSQL;
---
---

CREATE OR REPLACE FUNCTION blockRoom(
        roomid_ UUID,
        belongTo_ UUID
    ) RETURNS Boolean AS $$
DECLARE 
updateAt_holder TIMESTAMP;
isAdmin BOOLEAN :=FALSE;
BEGIN
    SELECT ROLE =1 INTO isAdmin  FROM users WHERE users.userid= belongTo_;
    IF EXISTS(
	 SELECT 1 FROM rooms WHERE roomid=roomid_
	 AND (belongTo <> belongTo_ or  isAdmin = FALSE )
	) THEN
     RAISE EXCEPTION 'only room owner can update room data';
	END IF ;

	-- IF EXISTS(
	--  SELECT 1 FROM rooms WHERE roomid=roomid_
	--  AND (isdeleted = TRUE )) THEN
 --     RAISE EXCEPTION 'لا يمكن اخفاء الغرفة لانها بالفعل محذوفة';
	-- END IF ;

	-- IF EXISTS(
	--  SELECT 1 FROM rooms WHERE roomid=roomid_
	--  AND (isblock = TRUE )) THEN
 --     RAISE EXCEPTION 'لا يمكن اخفاء الغرفة لانها بالفعل مخفية';
	-- END IF ;
	
    UPDATE rooms
    SET isblock = CASE
            WHEN isAdmin=TRUE AND isblock =FALSE 
			THEN TRUE ELSE FALSE
        END,
       	isdeleted=  CASE
            WHEN isAdmin = FALSE AND isdeleted =FALSE 
			THEN TRUE ELSE FALSE
        END,
        updateAt = CURRENT_TIMESTAMP
    WHERE roomid = roomid_;
    SELECT updateAt INTO updateAt_holder
    FROM rooms
    WHERE roomid = roomid_;

    RETURN TRUE;

EXCEPTION

    WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
    SQLERRM;
    RETURN FALSE;
END;
$$ LANGUAGE PLPGSQL;
---
---
CREATE OR REPLACE FUNCTION fn_room_update() RETURNS TRIGGER AS $$
DECLARE 
    isUserDeleted BOOLEAN;
BEGIN
    SELECT isdeleted INTO isUserDeleted
         FROM users
    WHERE userid = NEW.userid;

    IF isUserDeleted IS NOT NULL AND isUserDeleted = TRUE THEN 
		RETURN NEW;
    END IF;

    RETURN NULL;
EXCEPTION
    WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
    SQLERRM;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

--
----
----
----

CREATE TABLE Bookings (
    bookingID UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    roomID UUID NOT NULL  REFERENCES  rooms(roomid),
    belongTo UUID NOT NULL REFERENCES  users(userid),
    booking_start TIMESTAMP NOT NULL  CHECK (booking_start >= CURRENT_TIMESTAMP) ,
    booking_end TIMESTAMP NOT NULL  CHECK (booking_end > booking_start)  ,
    bookingStatus VARCHAR(50) NOT NULL CHECK (
        bookingStatus IN ('Pending', 'Confirmed', 'Cancelled')
    ) DEFAULT 'Confirmed',
    totalPrice NUMERIC(10, 2) NOT NULL CHECK (totalPrice > 0),
    servicePayment NUMERIC(10, 2)  NULL DEFAULT 0 CHECK (servicePayment >= 0),
    maintenancePayment NUMERIC(10, 2)  NULL DEFAULT 0 CHECK (maintenancePayment >= 0),
    paymentStatus VARCHAR(50) CHECK (
        paymentStatus IN ('Paid', 'Unpaid')
    ) DEFAULT 'Unpaid',
    createdAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    cancelledAt TIMESTAMP  DEFAULT NULL,
    cancellationReason TEXT DEFAULT NULL,
    actualCheckOut TIMESTAMP DEFAULT Null


);
----
----



----
----

CREATE OR REPLACE FUNCTION fn_isValid_booking(
startBooking TIMESTAMP,
endBooking TIMESTAMP,
belongTo_ UUID
)
RETURNS BOOLEAN AS $$
DECLARE
isValid BOOLEAN := FALSE;
BEGIN
 IF startBooking IS NULL OR endBooking IS NULL THEN
RAISE NOTICE 'Start and end booking dates cannot be NULL';
RETURN FALSE;
END IF;

IF (startBooking >= CURRENT_TIMESTAMP) = false THEN
        RAISE NOTICE 'Booking start date cannot be in the past';
        RETURN FALSE;
    END IF;

    IF (endBooking::date - startBooking::date) <1 THEN
      RAISE EXCEPTION 'Booking must at least one day ';
      RETURN FALSE;
    END IF;
SELECT COUNT(*) > 0 INTO isValid 
FROM bookings b
WHERE (startBooking, endBooking) OVERLAPS (b.booking_start, b.booking_end) 
AND (belongTo_ IS NULL OR belongTo_!=belongto)
AND bookingStatus='Confirmed'  ;
        RAISE NOTICE 'isbookingValide = %',isValid;

RETURN isValid=false;
EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN FALSE;
END;
$$ LANGUAGE plpgsql;
-----
-----

-----
-----
CREATE OR REPLACE FUNCTION fn_bookin_insert(
    roomid_ UUID,
    totalprice_ NUMERIC(10, 2),
    userid_ UUID,
    startbookindate_ TIMESTAMP,
    endbookingdate_ TIMESTAMP
) RETURNS BOOLEAN AS $$
DECLARE
    isNotDeletion Boolean;
BEGIN

IF startbookindate_ < CURRENT_TIMESTAMP THEN
        RAISE EXCEPTION 'Booking start date cannot be in the past';
        RETURN FALSE;
    END IF;

SELECT isdeleted INTO  isNotDeletion  FROM users WHERE userid = userid_ And isdeleted=false;

IF isNotDeletion = TRUE THEN
RAISE EXCEPTION 'The user is deleted';
RETURN FALSE;
END IF;

IF EXISTS (SELECT 1 FROM bookings b
WHERE (startbookindate_, endbookingdate_) OVERLAPS (b.booking_start, b.booking_end)
 AND b.bookingStatus='Confirmed')THEN
RETURN FALSE;
END IF;

INSERT INTO bookings(
    roomid,
    belongto,
    totalprice,
	booking_start,
	booking_end
	
) VALUES(
    roomid_,
    userid_,
    totalprice_,
    startbookindate_ ,
    endbookingdate_
);


RETURN TRUE;
EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN FALSE;
END;
$$ LANGUAGE plpgsql;
-----
-----

-----
-----

CREATE OR REPLACE FUNCTION fn_bookin_update(
    booking_id UUID,  
    totalprice_ NUMERIC(10, 2),
    userid_ UUID,
	bookingstatus_ VARCHAR,
    startbookindate_ TIMESTAMP,
    endbookingdate_ TIMESTAMP
) RETURNS BOOLEAN AS $$
DECLARE
    isNotDeletion Boolean;
BEGIN

IF startbookindate_ IS NOT NULL AND endbookingdate_ IS NOT NULL THEN 
	IF EXISTS (SELECT 1 FROM bookings b
			WHERE (startbookindate_, endbookingdate_) OVERLAPS (b.booking_start, b.booking_end)
			 AND b.bookingid<>booking_id AND b.bookingStatus='Confirmed')THEN
			RETURN FALSE;
	END IF;

	IF startbookindate_ < CURRENT_TIMESTAMP THEN
        RAISE EXCEPTION 'Booking start date cannot be in the past';
        RETURN FALSE;
    END IF;
END IF;



SELECT isdeleted INTO  isNotDeletion  FROM users WHERE userid = userid_ And isdeleted=false;

IF isNotDeletion = TRUE THEN
RAISE EXCEPTION 'The user is deleted';
RETURN FALSE;
END IF;


    UPDATE  bookings  SET 
        totalprice = CASE WHEN totalprice_ <> totalprice AND totalprice_ IS NOT NULL
            THEN totalprice_ ELSE totalprice END,
        booking_start = CASE WHEN startbookindate_ <> booking_start AND startbookindate_ IS NOT NULL
            THEN startbookindate_ ELSE booking_start END,
        booking_end =  CASE WHEN endbookingdate_ <> booking_end AND endbookingdate_ IS NOT NULL
            THEN endbookingdate_ ELSE booking_end END,
        bookingstatus =  CASE WHEN bookingstatus_ <> bookingstatus AND bookingstatus_ IS NOT NULL
            THEN bookingstatus_ ELSE bookingstatus END
        WHERE bookingid = booking_id;

    RETURN TRUE;
EXCEPTION
    WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
    SQLERRM;
    RETURN FALSE;
END;
$$ LANGUAGE plpgsql;
-----
-----


-----
----- 
CREATE OR REPLACE FUNCTION fun_get_list_of_booking_at_specific_month_and_year
(
year_ INT,
month_ INT,
bookingID UUID
)
RETURNS TEXT
AS $$
DECLARE
startAt_ INT :=0;
maxDayAtMon_ INT:=0;
temp_date DATE;
dayes_ TEXT :='';
BEGIN
   IF month_>12 OR month_<1 THEN 
   RAISE NOTICE 'month is not valide ';
   RETURN '';
   END IF ;

    maxDayAtMon_ := EXTRACT(DAY FROM 
        (MAKE_DATE(year_, month_, 1) + INTERVAL '1 month - 1 day'));

FOR day_num in 1..maxDayAtMon_ LOOP
 
   IF EXISTS (
  SELECT 1 FROM bookings 
  WHERE 
 	 (
	  		MAKE_DATE(year_, month_, day_num) = booking_start::DATE OR
   			MAKE_DATE(year_, month_, day_num) = booking_end::DATE
	 ) 
  OR
    (
	        MAKE_DATE(year_, month_, day_num)
            BETWEEN booking_start::DATE AND booking_end::DATE
		  
    ) AND
	(
	bookingID IS NULL OR bookingID!=bookingID

	) AND bookingStatus='Confirmed'
 ) THEN 
     RAISE NOTICE 'THIS THE generatedDate =% ',
   MAKE_DATE(year_, month_, day_num) ;
   dayes_ := dayes_ || ', ' || day_num::TEXT;
  END IF ; 
END LOOP;

RETURN  ltrim(dayes_, ',');

EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN '';
END;
$$LANGUAGE plpgsql;
-----
-----




----
----

CREATE OR REPLACE FUNCTION fun_getBookingPagination(
    belongId UUID,
    pageNumber INT, 
    limitNumber INT
    )
RETURNS  TABLE (
     bookingID UUID ,
    roomID UUID  ,
    belongTo UUID  ,
    booking_start TIMESTAMP   ,
    booking_end TIMESTAMP   ,
    bookingStatus VARCHAR(50) ,
    totalPrice NUMERIC(10, 2) ,
    servicePayment NUMERIC(10, 2) ,
    maintenancePayment NUMERIC(10, 2)  ,
    paymentStatus VARCHAR(50)  ,
    createdAt TIMESTAMP    ,
    cancelledAt TIMESTAMP  ,
    cancellationReason TEXT  ,
    actualCheckOut TIMESTAMP  
)
AS $$
BEGIN

RETURN QUERY SELECT 
    b.bookingID ,
    b.roomID   ,
    b.belongTo   ,
    b.booking_start   ,
    b.booking_end   ,
    b.bookingStatus  ,
    b.totalPrice  ,
    b.servicePayment ,
    b.maintenancePayment   ,
    b.paymentStatus   ,
    b.createdAt  ,
    b.cancelledAt   ,
    b.cancellationReason   ,
    b.actualCheckOut   
FROM bookings b
WHERE  (belongId IS NULL AND b.belongTo IS NOT NULL)   OR  b.belongTo = belongId 
ORDER BY b.createdAt DESC
LIMIT limitNumber OFFSET limitNumber * (pageNumber - 1);
EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN QUERY 
SELECT
    NULL   ,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL;

END;
$$LANGUAGE plpgsql;
----
----

CREATE OR REPLACE FUNCTION fun_getBookingBelongToUserPagination(
    Userid UUID,
    pageNumber INT, 
    limitNumber INT
    )
RETURNS  TABLE (
     bookingID UUID ,
    roomID UUID  ,
    belongTo UUID  ,
    booking_start TIMESTAMP   ,
    booking_end TIMESTAMP   ,
    bookingStatus VARCHAR(50) ,
    totalPrice NUMERIC(10, 2) ,
    servicePayment NUMERIC(10, 2) ,
    maintenancePayment NUMERIC(10, 2)  ,
    paymentStatus VARCHAR(50)  ,
    createdAt TIMESTAMP    ,
    cancelledAt TIMESTAMP  ,
    cancellationReason TEXT  ,
    actualCheckOut TIMESTAMP  
)
AS $$
BEGIN

RETURN QUERY SELECT 
    b.bookingID ,
    b.roomID   ,
    b.belongTo   ,
    b.booking_start   ,
    b.booking_end   ,
    b.bookingStatus  ,
    b.totalPrice  ,
    b.servicePayment ,
    b.maintenancePayment   ,
    b.paymentStatus   ,
    b.createdAt  ,
    b.cancelledAt   ,
    b.cancellationReason   ,
    b.actualCheckOut   
FROM bookings b
INNER JOIN rooms ro 
ON b.roomid = ro.roomid
WHERE  ro.belongto =  Userid
ORDER BY b.createdAt DESC
LIMIT limitNumber OFFSET limitNumber * (pageNumber - 1);
EXCEPTION
WHEN OTHERS THEN RAISE EXCEPTION 'Something went wrong: %',
SQLERRM;
RETURN QUERY 
SELECT
    NULL   ,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL;

END;
$$LANGUAGE plpgsql;
