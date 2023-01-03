-- You may want to check this script before running it as admin

-- Create db with:
-- CREATE DATABASE cuzzledb TEMPLATE template1;

-- Then run this command as postgres user
-- psql -U cuzzle_user -d cuzzledb -a -f init_db.sql

-- Create badge
CREATE TYPE badge_rarity AS ENUM ('stone', 'silver', 'gold', 'platinum');

-- Enable UUID generation
CREATE EXTENSION if not exists "pgcrypto";

CREATE TABLE IF NOT EXISTS badge(
    id UUID PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
    description varchar(128) NULL,
    name varchar(32) NOT NULL,
    rarity badge_rarity NOT NULL
);

-- Create account
CREATE TABLE IF NOT EXISTS account(
    id UUID PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
    created_at date NOT NULL default CURRENT_DATE,
    days_entered smallint NOT NULL default 1,
    email varchar(256) UNIQUE NOT NULL,
    email_confirmed boolean default false,
    password_hash bytea NOT NULL,
    password_salt bytea NOT NULL,
    username varchar(64) NOT NULL,
    refresh_token bytea NULL,
    refresh_token_expire_date DATE NULL
);

-- Create user badge
CREATE TABLE IF NOT EXISTS user_badge(
    reception_date DATE default CURRENT_DATE,
    badge_id UUID references badge(id),
    account_id UUID references account(id)
);

-- Create solution
CREATE TABLE IF NOT EXISTS solution(
    id UUID PRIMARY KEY NOT NULL default get_random_uuid(),
    content TEXT NULL,
    points SMALLINT NULL,
    account_id UUID NULL references account(id)
);

-- Create puzzle
CREATE TABLE IF NOT EXISTS puzzle(
    id UUID PRIMARY KEY NOT NULL default get_random_uuid(),
    allow_anonymous boolean default false,
    created_at date default CURRENT_DATE,
    flag varchar(32) null,
    is_published boolean default false,
    last_modified date default CURRENT_DATE,
    name varchar(64) NOT NULL,
    question text null,
    solution_id UUID references solution(id),
    account_id UUID references account(id)
);

-- Create bookmark
CREATE TABLE IF NOT EXISTS bookmark(
    created_at date default CURRENT_DATE,
    account_id UUID references account(id),
    puzzle_id UUID references puzzle(id)
);

-- Grant some permissions to user
GRANT CONNECT ON DATABASE cuzzledb TO cuzzle_user;
GRANT SELECT, INSERT, UPDATE ON TABLE account TO cuzzle_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE puzzle TO cuzzle_user;
