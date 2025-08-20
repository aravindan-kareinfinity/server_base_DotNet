# Database Table Creation Scripts

## PostgreSQL Database Schema for PlanItNoww Authentication System

### 1. Users Table
```sql
CREATE TABLE Users (
    id BIGSERIAL PRIMARY KEY,
    email VARCHAR(255),
    mobile VARCHAR(20),
    fullname VARCHAR(255),
    dateofbirth DATE,
    gender VARCHAR(10),
    profilepicture TEXT,
    passwordhash VARCHAR(255),
    salt VARCHAR(255),
    googleid VARCHAR(255),
    facebookid VARCHAR(255),
    roleid BIGINT DEFAULT 1,
    pushnotificationtoken TEXT,
    isemailverified BOOLEAN DEFAULT FALSE,
    ismobileverified BOOLEAN DEFAULT FALSE,
    isaadhaarverified BOOLEAN DEFAULT FALSE,
    isactive BOOLEAN DEFAULT TRUE,
    issuspended BOOLEAN DEFAULT FALSE,
    accesstoken TEXT,
    refreshtoken TEXT,
    version INTEGER DEFAULT 1,
    notes TEXT,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for Users table
CREATE INDEX idx_users_email ON Users(email);
CREATE INDEX idx_users_mobile ON Users(mobile);
CREATE INDEX idx_users_googleid ON Users(googleid);
CREATE INDEX idx_users_facebookid ON Users(facebookid);
CREATE INDEX idx_users_roleid ON Users(roleid);
CREATE INDEX idx_users_isactive ON Users(isactive);
CREATE INDEX idx_users_createdon ON Users(createdon);
```

### 2. OTPs Table
```sql
CREATE TABLE OTPs (
    id BIGSERIAL PRIMARY KEY,
    userid BIGINT,
    emailormobile VARCHAR(255),
    mobile VARCHAR(20),
    email VARCHAR(255),
    otpcode VARCHAR(10) NOT NULL,
    purpose VARCHAR(50) DEFAULT 'LOGIN',
    expiresat TIMESTAMP NOT NULL,
    isused BOOLEAN DEFAULT FALSE,
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    notes TEXT,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for OTPs table
CREATE INDEX idx_otps_emailormobile ON OTPs(emailormobile);
CREATE INDEX idx_otps_mobile ON OTPs(mobile);
CREATE INDEX idx_otps_email ON OTPs(email);
CREATE INDEX idx_otps_otpcode ON OTPs(otpcode);
CREATE INDEX idx_otps_purpose ON OTPs(purpose);
CREATE INDEX idx_otps_expiresat ON OTPs(expiresat);
CREATE INDEX idx_otps_isused ON OTPs(isused);
CREATE INDEX idx_otps_isactive ON OTPs(isactive);
```

### 3. UserSessions Table
```sql
CREATE TABLE UserSessions (
    id BIGSERIAL PRIMARY KEY,
    userid BIGINT NOT NULL,
    code VARCHAR(255) NOT NULL,
    starttime TIMESTAMP NOT NULL,
    endtime TIMESTAMP NOT NULL,
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    notes TEXT,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for UserSessions table
CREATE INDEX idx_usersessions_userid ON UserSessions(userid);
CREATE INDEX idx_usersessions_code ON UserSessions(code);
CREATE INDEX idx_usersessions_starttime ON UserSessions(starttime);
CREATE INDEX idx_usersessions_endtime ON UserSessions(endtime);
CREATE INDEX idx_usersessions_isactive ON UserSessions(isactive);
```

### 4. Roles Table
```sql
CREATE TABLE Roles (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    notes TEXT,
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for Roles table
CREATE INDEX idx_roles_name ON Roles(name);
CREATE INDEX idx_roles_isactive ON Roles(isactive);
```

### 5. PermissionsReference Table
```sql
CREATE TABLE PermissionsReference (
    id BIGSERIAL PRIMARY KEY,
    roleid BIGINT NOT NULL,
    description TEXT,
    name VARCHAR(100) NOT NULL,
    notes TEXT,
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for PermissionsReference table
CREATE INDEX idx_permissionsreference_roleid ON PermissionsReference(roleid);
CREATE INDEX idx_permissionsreference_name ON PermissionsReference(name);
CREATE INDEX idx_permissionsreference_isactive ON PermissionsReference(isactive);
```

### 6. RefreshTokens Table
```sql
CREATE TABLE RefreshTokens (
    id BIGSERIAL PRIMARY KEY,
    token VARCHAR(500) NOT NULL,
    userid BIGINT NOT NULL,
    expiresat TIMESTAMP NOT NULL,
    notes TEXT,
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for RefreshTokens table
CREATE INDEX idx_refreshtokens_token ON RefreshTokens(token);
CREATE INDEX idx_refreshtokens_userid ON RefreshTokens(userid);
CREATE INDEX idx_refreshtokens_expiresat ON RefreshTokens(expiresat);
CREATE INDEX idx_refreshtokens_isactive ON RefreshTokens(isactive);
```

### 7. UserProfiles Table
```sql
CREATE TABLE UserProfiles (
    id BIGSERIAL PRIMARY KEY,
    userid BIGINT NOT NULL,
    firstname VARCHAR(100),
    lastname VARCHAR(100),
    middlename VARCHAR(100),
    address TEXT,
    city VARCHAR(100),
    state VARCHAR(100),
    country VARCHAR(100),
    postalcode VARCHAR(20),
    phone VARCHAR(20),
    dateofbirth DATE,
    gender VARCHAR(10),
    profilepicture TEXT,
    bio TEXT,
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    notes TEXT,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for UserProfiles table
CREATE INDEX idx_userprofiles_userid ON UserProfiles(userid);
CREATE INDEX idx_userprofiles_isactive ON UserProfiles(isactive);
```

### 8. Files Table
```sql
CREATE TABLE Files (
    id BIGSERIAL PRIMARY KEY,
    type VARCHAR(50) NOT NULL,
    content BYTEA,
    filename VARCHAR(255),
    filepath TEXT,
    filesize BIGINT,
    contenttype VARCHAR(100),
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    notes TEXT,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for Files table
CREATE INDEX idx_files_type ON Files(type);
CREATE INDEX idx_files_filename ON Files(filename);
CREATE INDEX idx_files_isactive ON Files(isactive);
```

### 9. Payments Table
```sql
CREATE TABLE Payments (
    id BIGSERIAL PRIMARY KEY,
    userid BIGINT NOT NULL,
    paymentgateway VARCHAR(50) NOT NULL,
    paymentid VARCHAR(255) NOT NULL,
    orderid VARCHAR(255) NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    currency VARCHAR(3) DEFAULT 'INR',
    status VARCHAR(50) NOT NULL,
    failurereason TEXT,
    refundstatus VARCHAR(50),
    receipturl TEXT,
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    notes TEXT,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for Payments table
CREATE INDEX idx_payments_userid ON Payments(userid);
CREATE INDEX idx_payments_paymentid ON Payments(paymentid);
CREATE INDEX idx_payments_orderid ON Payments(orderid);
CREATE INDEX idx_payments_status ON Payments(status);
CREATE INDEX idx_payments_isactive ON Payments(isactive);
```

### 10. Subscriptions Table
```sql
CREATE TABLE Subscriptions (
    id BIGSERIAL PRIMARY KEY,
    userid BIGINT NOT NULL,
    planid BIGINT NOT NULL,
    startdate DATE NOT NULL,
    enddate DATE NOT NULL,
    status VARCHAR(50) NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    currency VARCHAR(3) DEFAULT 'INR',
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    notes TEXT,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for Subscriptions table
CREATE INDEX idx_subscriptions_userid ON Subscriptions(userid);
CREATE INDEX idx_subscriptions_planid ON Subscriptions(planid);
CREATE INDEX idx_subscriptions_status ON Subscriptions(status);
CREATE INDEX idx_subscriptions_isactive ON Subscriptions(isactive);
```

### 11. ReferenceType Table
```sql
CREATE TABLE ReferenceType (
    id BIGSERIAL PRIMARY KEY,
    identifier VARCHAR(100) NOT NULL,
    displaytext VARCHAR(255) NOT NULL,
    langcode VARCHAR(10) DEFAULT 'en',
    parentid BIGINT,
    sortorder INTEGER DEFAULT 0,
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    notes TEXT,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for ReferenceType table
CREATE INDEX idx_referencetype_identifier ON ReferenceType(identifier);
CREATE INDEX idx_referencetype_parentid ON ReferenceType(parentid);
CREATE INDEX idx_referencetype_isactive ON ReferenceType(isactive);
```

### 12. ReferenceValue Table
```sql
CREATE TABLE ReferenceValue (
    id BIGSERIAL PRIMARY KEY,
    referencetypeid BIGINT NOT NULL,
    identifier VARCHAR(100) NOT NULL,
    displaytext VARCHAR(255) NOT NULL,
    langcode VARCHAR(10) DEFAULT 'en',
    parentid BIGINT,
    sortorder INTEGER DEFAULT 0,
    isactive BOOLEAN DEFAULT TRUE,
    version INTEGER DEFAULT 1,
    notes TEXT,
    createdby BIGINT,
    createdon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modifiedby BIGINT,
    modifiedon TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    attributes JSONB DEFAULT '{}'
);

-- Indexes for ReferenceValue table
CREATE INDEX idx_referencevalue_referencetypeid ON ReferenceValue(referencetypeid);
CREATE INDEX idx_referencevalue_identifier ON ReferenceValue(identifier);
CREATE INDEX idx_referencevalue_parentid ON ReferenceValue(parentid);
CREATE INDEX idx_referencevalue_isactive ON ReferenceValue(isactive);
```

## Foreign Key Constraints

```sql
-- Users table foreign keys
ALTER TABLE Users ADD CONSTRAINT fk_users_roleid FOREIGN KEY (roleid) REFERENCES Roles(id);

-- OTPs table foreign keys
ALTER TABLE OTPs ADD CONSTRAINT fk_otps_userid FOREIGN KEY (userid) REFERENCES Users(id);

-- UserSessions table foreign keys
ALTER TABLE UserSessions ADD CONSTRAINT fk_usersessions_userid FOREIGN KEY (userid) REFERENCES Users(id);

-- PermissionsReference table foreign keys
ALTER TABLE PermissionsReference ADD CONSTRAINT fk_permissionsreference_roleid FOREIGN KEY (roleid) REFERENCES Roles(id);

-- RefreshTokens table foreign keys
ALTER TABLE RefreshTokens ADD CONSTRAINT fk_refreshtokens_userid FOREIGN KEY (userid) REFERENCES Users(id);

-- UserProfiles table foreign keys
ALTER TABLE UserProfiles ADD CONSTRAINT fk_userprofiles_userid FOREIGN KEY (userid) REFERENCES Users(id);

-- Payments table foreign keys
ALTER TABLE Payments ADD CONSTRAINT fk_payments_userid FOREIGN KEY (userid) REFERENCES Users(id);

-- Subscriptions table foreign keys
ALTER TABLE Subscriptions ADD CONSTRAINT fk_subscriptions_userid FOREIGN KEY (userid) REFERENCES Users(id);

-- ReferenceType table foreign keys
ALTER TABLE ReferenceType ADD CONSTRAINT fk_referencetype_parentid FOREIGN KEY (parentid) REFERENCES ReferenceType(id);

-- ReferenceValue table foreign keys
ALTER TABLE ReferenceValue ADD CONSTRAINT fk_referencevalue_referencetypeid FOREIGN KEY (referencetypeid) REFERENCES ReferenceType(id);
ALTER TABLE ReferenceValue ADD CONSTRAINT fk_referencevalue_parentid FOREIGN KEY (parentid) REFERENCES ReferenceValue(id);
```

## Initial Data Insertion

### Insert Default Roles
```sql
INSERT INTO Roles (name, description, notes, isactive, version, createdby, createdon, modifiedby, modifiedon) VALUES
('Admin', 'Administrator with full access', 'System administrator role', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
('User', 'Standard user with basic access', 'Default user role', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
('Moderator', 'Moderator with limited admin access', 'Content moderation role', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP);
```

### Insert Default Reference Types
```sql
INSERT INTO ReferenceType (identifier, displaytext, langcode, isactive, version, createdby, createdon, modifiedby, modifiedon) VALUES
('GENDER', 'Gender', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
('USER_STATUS', 'User Status', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
('OTP_PURPOSE', 'OTP Purpose', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP);
```

### Insert Default Reference Values
```sql
INSERT INTO ReferenceValue (referencetypeid, identifier, displaytext, langcode, isactive, version, createdby, createdon, modifiedby, modifiedon) VALUES
(1, 'MALE', 'Male', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
(1, 'FEMALE', 'Female', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
(1, 'OTHER', 'Other', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
(2, 'ACTIVE', 'Active', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
(2, 'INACTIVE', 'Inactive', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
(2, 'SUSPENDED', 'Suspended', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
(3, 'SIGNUP', 'Sign Up', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
(3, 'LOGIN', 'Login', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
(3, 'PASSWORD_RESET', 'Password Reset', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP),
(3, 'VERIFICATION', 'Verification', 'en', true, 1, 1, CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP);
```

## Database Configuration

### Enable Required Extensions
```sql
-- Enable JSONB support (if not already enabled)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Enable pgcrypto for cryptographic functions
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
```

### Create Database User (Optional)
```sql
-- Create a dedicated user for the application
CREATE USER planitnoww_user WITH PASSWORD 'your_secure_password';

-- Grant necessary permissions
GRANT CONNECT ON DATABASE your_database_name TO planitnoww_user;
GRANT USAGE ON SCHEMA public TO planitnoww_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO planitnoww_user;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO planitnoww_user;

-- Grant permissions for future tables
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO planitnoww_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT ON SEQUENCES TO planitnoww_user;
```

## Notes

1. **Database Version**: This schema is designed for PostgreSQL 12+ with JSONB support
2. **Security**: All passwords are stored as hashed values with salt
3. **Audit Trail**: All tables include createdby, createdon, modifiedby, modifiedon fields for audit purposes
4. **Soft Delete**: Records are marked as inactive rather than physically deleted
5. **Versioning**: All tables include version field for optimistic concurrency control
6. **Indexes**: Essential indexes are created for performance on frequently queried fields
7. **Foreign Keys**: Proper referential integrity is maintained through foreign key constraints

## Usage Instructions

1. **Create Database**: First create a PostgreSQL database
2. **Run Schema Scripts**: Execute the table creation scripts in order
3. **Insert Initial Data**: Run the initial data insertion scripts
4. **Configure Application**: Update your `appsettings.json` with the database connection string
5. **Test Connection**: Verify the application can connect to the database

## Connection String Format
```
Host=localhost;Port=5432;Database=planitnoww_db;Username=planitnoww_user;Password=your_secure_password
```
