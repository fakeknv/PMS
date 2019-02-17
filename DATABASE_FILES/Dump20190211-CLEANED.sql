-- MySQL dump 10.13  Distrib 5.7.23, for Win64 (x86_64)
--
-- Host: localhost    Database: pms_db
-- ------------------------------------------------------
-- Server version	5.7.23-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account_logs`
--

DROP TABLE IF EXISTS `account_logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `account_logs` (
  `log_id` varchar(11) NOT NULL,
  `account_id` varchar(15) NOT NULL,
  `log_details` varchar(450) NOT NULL,
  `log_time` time NOT NULL,
  `log_date` date NOT NULL,
  PRIMARY KEY (`log_id`),
  KEY `FK_17_idx` (`account_id`),
  CONSTRAINT `FK_17` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account_logs`
--

LOCK TABLES `account_logs` WRITE;
/*!40000 ALTER TABLE `account_logs` DISABLE KEYS */;
INSERT INTO `account_logs` VALUES ('LOG-1','ACNT-2','Account Logged In','21:30:31','2019-02-11'),('LOG-2','ACNT-2','Account Logged In','21:37:27','2019-02-11');
/*!40000 ALTER TABLE `account_logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `accounts`
--

DROP TABLE IF EXISTS `accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `accounts` (
  `account_id` varchar(15) NOT NULL,
  `user_name` varchar(45) NOT NULL,
  `pass_key` varchar(85) NOT NULL,
  `account_type` int(5) NOT NULL,
  PRIMARY KEY (`account_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts`
--

LOCK TABLES `accounts` WRITE;
/*!40000 ALTER TABLE `accounts` DISABLE KEYS */;
INSERT INTO `accounts` VALUES ('ACNT-2','registrar','$MYHASH$V1$10000$nKgMVwkS75gM8xlAE4ShKwca88aI6rGb63IeMHe32XiD7Vqe',4),('ACNT-3','cashier','$MYHASH$V1$10000$tmZakePsaRQ0uLiDmHN6mbjrfSpGdYRL2gAl6DJENgulkX4T',3),('ACNT-4','secretary','$MYHASH$V1$10000$5+h7ZtmPZreeb6VmLxEeO/CD9ZRkPVZYl/znVIBZHhQanVoj',2),('prms-0000','admin','$MYHASH$V1$10000$ubSayW65gBRd4KrlYaq2ykc4MlLl9iDbZG9cXWE7nqLlhYub',1);
/*!40000 ALTER TABLE `accounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `accounts_info`
--

DROP TABLE IF EXISTS `accounts_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `accounts_info` (
  `account_id` varchar(15) NOT NULL,
  `name` varchar(150) NOT NULL,
  `date_created` date NOT NULL,
  `time_created` time NOT NULL,
  `creator` varchar(15) NOT NULL,
  PRIMARY KEY (`account_id`),
  CONSTRAINT `FK_1` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts_info`
--

LOCK TABLES `accounts_info` WRITE;
/*!40000 ALTER TABLE `accounts_info` DISABLE KEYS */;
INSERT INTO `accounts_info` VALUES ('ACNT-2','registrar','2019-01-31','20:51:35','prms-0000'),('ACNT-3','cashier','2019-02-01','20:28:15','prms-0000'),('ACNT-4','secretary','2019-02-01','20:28:59','prms-0000'),('prms-0000','John W. Doe','2018-01-01','12:00:00','prms-0000');
/*!40000 ALTER TABLE `accounts_info` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appointment_types`
--

DROP TABLE IF EXISTS `appointment_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `appointment_types` (
  `type_id` varchar(15) NOT NULL,
  `appointment_type` varchar(250) NOT NULL,
  `custom` varchar(25) NOT NULL,
  `fee` double NOT NULL,
  `status` int(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appointment_types`
--

LOCK TABLES `appointment_types` WRITE;
/*!40000 ALTER TABLE `appointment_types` DISABLE KEYS */;
INSERT INTO `appointment_types` VALUES ('AT-1','Thanksgiving Mass','1',101,1),('AT-2','Petition Mass','1',102,1),('AT-3','Special Intention','1',103,1),('AT-4','All Souls','1',104,1),('AT-5','Soul/s of','1',105,1),('AT-6','Sick Call','2',106,1),('AT-7','Blessing','2',107,1),('AT-8','Funeral Mass','1',1234,1);
/*!40000 ALTER TABLE `appointment_types` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appointments`
--

DROP TABLE IF EXISTS `appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `appointments` (
  `appointment_id` varchar(15) NOT NULL,
  `appointment_date` date NOT NULL,
  `appointment_time` time NOT NULL,
  `appointment_type` varchar(15) NOT NULL DEFAULT '----',
  `requested_by` varchar(150) NOT NULL,
  `placed_by` varchar(15) NOT NULL,
  `remarks` varchar(350) NOT NULL DEFAULT 'NA',
  `status` int(15) NOT NULL DEFAULT '1',
  `assigned_priest` varchar(15) NOT NULL DEFAULT 'NA',
  PRIMARY KEY (`appointment_id`),
  KEY `FK_6_idx` (`placed_by`),
  KEY `FK_14_idx` (`appointment_type`),
  KEY `FK_15_idx` (`assigned_priest`),
  CONSTRAINT `FK_14` FOREIGN KEY (`appointment_type`) REFERENCES `appointment_types` (`type_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_15` FOREIGN KEY (`assigned_priest`) REFERENCES `residing_priests` (`priest_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_2` FOREIGN KEY (`placed_by`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appointments`
--

LOCK TABLES `appointments` WRITE;
/*!40000 ALTER TABLE `appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `archives`
--

DROP TABLE IF EXISTS `archives`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `archives` (
  `book_number` int(100) NOT NULL,
  `date_archived` date NOT NULL,
  `time_archived` time NOT NULL,
  `archived_by` varchar(15) NOT NULL,
  PRIMARY KEY (`book_number`),
  KEY `FK_4_idx` (`archived_by`),
  CONSTRAINT `FK_3` FOREIGN KEY (`book_number`) REFERENCES `registers` (`book_number`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_4` FOREIGN KEY (`archived_by`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `archives`
--

LOCK TABLES `archives` WRITE;
/*!40000 ALTER TABLE `archives` DISABLE KEYS */;
/*!40000 ALTER TABLE `archives` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `baptismal_records`
--

DROP TABLE IF EXISTS `baptismal_records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `baptismal_records` (
  `record_id` varchar(1000) NOT NULL,
  `birthday` date NOT NULL,
  `legitimacy` varchar(150) NOT NULL DEFAULT 'Unknown',
  `place_of_birth` varchar(400) NOT NULL DEFAULT 'Unknown',
  `sponsor1` varchar(150) NOT NULL DEFAULT 'Unknown',
  `sponsor2` varchar(150) NOT NULL DEFAULT 'Unknown',
  `stipend` float NOT NULL DEFAULT '0',
  `minister` varchar(150) NOT NULL DEFAULT 'Unknown',
  `remarks` varchar(500) NOT NULL DEFAULT 'None.',
  PRIMARY KEY (`record_id`),
  KEY `ID_1` (`remarks`,`minister`,`stipend`,`sponsor2`,`sponsor1`,`birthday`),
  CONSTRAINT `FK_5` FOREIGN KEY (`record_id`) REFERENCES `records` (`record_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `baptismal_records`
--

LOCK TABLES `baptismal_records` WRITE;
/*!40000 ALTER TABLE `baptismal_records` DISABLE KEYS */;
/*!40000 ALTER TABLE `baptismal_records` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `burial_directory`
--

DROP TABLE IF EXISTS `burial_directory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `burial_directory` (
  `directory_id` varchar(15) NOT NULL,
  `record_id` varchar(15) NOT NULL,
  `block` varchar(250) NOT NULL DEFAULT 'Not Specified.',
  `lot` varchar(250) NOT NULL DEFAULT 'Not Specified.',
  `plot` varchar(250) NOT NULL DEFAULT 'Not Specified.',
  `gravestone` longblob,
  `relative_contact_number` varchar(50) NOT NULL DEFAULT 'Not Specified.',
  PRIMARY KEY (`directory_id`),
  KEY `FK_16_idx` (`record_id`),
  CONSTRAINT `FK_16` FOREIGN KEY (`record_id`) REFERENCES `records` (`record_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `burial_directory`
--

LOCK TABLES `burial_directory` WRITE;
/*!40000 ALTER TABLE `burial_directory` DISABLE KEYS */;
/*!40000 ALTER TABLE `burial_directory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `burial_records`
--

DROP TABLE IF EXISTS `burial_records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `burial_records` (
  `record_id` varchar(1000) NOT NULL,
  `burial_date` date NOT NULL,
  `age` int(200) NOT NULL DEFAULT '0',
  `status` varchar(150) NOT NULL DEFAULT 'Unknown',
  `residence` varchar(400) NOT NULL DEFAULT 'Unknown',
  `residence2` varchar(400) NOT NULL DEFAULT 'Unknown',
  `sacrament` varchar(200) NOT NULL DEFAULT 'Unknown',
  `cause_of_death` varchar(150) NOT NULL DEFAULT 'Unknown',
  `place_of_interment` varchar(400) NOT NULL DEFAULT 'Unknown',
  `stipend` float NOT NULL DEFAULT '0',
  `minister` varchar(150) NOT NULL DEFAULT 'Unknown',
  `remarks` varchar(500) NOT NULL DEFAULT 'None.',
  PRIMARY KEY (`record_id`),
  CONSTRAINT `FK_6` FOREIGN KEY (`record_id`) REFERENCES `records` (`record_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `burial_records`
--

LOCK TABLES `burial_records` WRITE;
/*!40000 ALTER TABLE `burial_records` DISABLE KEYS */;
/*!40000 ALTER TABLE `burial_records` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `confirmation_records`
--

DROP TABLE IF EXISTS `confirmation_records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `confirmation_records` (
  `record_id` varchar(1000) NOT NULL,
  `age` int(200) NOT NULL DEFAULT '0',
  `parochia` varchar(150) NOT NULL DEFAULT 'Unknown',
  `province` varchar(150) NOT NULL DEFAULT 'Unknown',
  `place_of_baptism` varchar(400) NOT NULL DEFAULT 'Unknown',
  `sponsor` varchar(100) NOT NULL DEFAULT 'Unknown',
  `sponsor2` varchar(100) NOT NULL DEFAULT 'Unknown',
  `stipend` float NOT NULL DEFAULT '0',
  `minister` varchar(100) NOT NULL DEFAULT 'Unknown',
  `remarks` varchar(500) NOT NULL DEFAULT 'None.',
  PRIMARY KEY (`record_id`),
  KEY `index2` (`remarks`,`minister`,`sponsor2`,`sponsor`),
  CONSTRAINT `FK_7` FOREIGN KEY (`record_id`) REFERENCES `records` (`record_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `confirmation_records`
--

LOCK TABLES `confirmation_records` WRITE;
/*!40000 ALTER TABLE `confirmation_records` DISABLE KEYS */;
/*!40000 ALTER TABLE `confirmation_records` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `matrimonial_records`
--

DROP TABLE IF EXISTS `matrimonial_records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `matrimonial_records` (
  `record_id` varchar(1000) NOT NULL,
  `recordholder2_fullname` varchar(150) NOT NULL DEFAULT 'Unknown',
  `parent1_fullname2` varchar(150) NOT NULL DEFAULT 'Unknown',
  `parent2_fullname2` varchar(150) NOT NULL DEFAULT 'Unknown',
  `status1` varchar(150) NOT NULL DEFAULT 'Unknown',
  `status2` varchar(150) NOT NULL DEFAULT 'Unknown',
  `age1` int(200) NOT NULL DEFAULT '0',
  `age2` int(200) NOT NULL DEFAULT '0',
  `place_of_origin1` varchar(400) NOT NULL DEFAULT 'Unknown',
  `place_of_origin2` varchar(400) NOT NULL DEFAULT 'Unknown',
  `residence1` varchar(400) NOT NULL DEFAULT 'Unknown',
  `residence2` varchar(400) NOT NULL DEFAULT 'Unknown',
  `witness1` varchar(150) NOT NULL DEFAULT 'Unknown',
  `witness2` varchar(150) NOT NULL DEFAULT 'Unknown',
  `witness1address` varchar(400) NOT NULL DEFAULT 'Unknown',
  `witness2address` varchar(400) NOT NULL DEFAULT 'Unknown',
  `stipend` float NOT NULL DEFAULT '0',
  `minister` varchar(100) NOT NULL DEFAULT 'Unknown',
  `remarks` varchar(500) NOT NULL DEFAULT 'None.',
  PRIMARY KEY (`record_id`),
  CONSTRAINT `FK_8` FOREIGN KEY (`record_id`) REFERENCES `records` (`record_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `matrimonial_records`
--

LOCK TABLES `matrimonial_records` WRITE;
/*!40000 ALTER TABLE `matrimonial_records` DISABLE KEYS */;
/*!40000 ALTER TABLE `matrimonial_records` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `records`
--

DROP TABLE IF EXISTS `records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `records` (
  `record_id` varchar(1000) NOT NULL,
  `book_number` int(15) NOT NULL,
  `page_number` int(200) NOT NULL,
  `entry_number` int(10) NOT NULL,
  `record_date` date NOT NULL,
  `recordholder_fullname` varchar(200) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `parent1_fullname` varchar(200) NOT NULL,
  `parent2_fullname` varchar(200) NOT NULL,
  PRIMARY KEY (`record_id`),
  KEY `records_idx_number_number` (`book_number`,`page_number`),
  KEY `index3` (`parent2_fullname`,`parent1_fullname`,`recordholder_fullname`,`record_date`),
  CONSTRAINT `FK_9` FOREIGN KEY (`book_number`) REFERENCES `registers` (`book_number`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `records`
--

LOCK TABLES `records` WRITE;
/*!40000 ALTER TABLE `records` DISABLE KEYS */;
/*!40000 ALTER TABLE `records` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `records_log`
--

DROP TABLE IF EXISTS `records_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `records_log` (
  `log_id` varchar(15) NOT NULL,
  `log_code` varchar(45) NOT NULL,
  `log_date` date NOT NULL,
  `log_time` time NOT NULL,
  `log_creator` varchar(15) NOT NULL,
  `record_id` varchar(15) NOT NULL,
  PRIMARY KEY (`log_id`),
  KEY `FK_record_id4_idx` (`record_id`),
  KEY `FK_account_id1_idx` (`log_creator`),
  CONSTRAINT `FK_10` FOREIGN KEY (`log_creator`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_11` FOREIGN KEY (`record_id`) REFERENCES `records` (`record_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `records_log`
--

LOCK TABLES `records_log` WRITE;
/*!40000 ALTER TABLE `records_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `records_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `registers`
--

DROP TABLE IF EXISTS `registers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `registers` (
  `book_number` int(100) NOT NULL,
  `register_number` int(10) NOT NULL,
  `book` varchar(150) DEFAULT NULL,
  `book_type` varchar(100) NOT NULL,
  `creation_date` date NOT NULL,
  `addition_date` date NOT NULL,
  `addition_time` time NOT NULL,
  `status` varchar(45) NOT NULL DEFAULT 'Normal',
  PRIMARY KEY (`book_number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `registers`
--

LOCK TABLES `registers` WRITE;
/*!40000 ALTER TABLE `registers` DISABLE KEYS */;
/*!40000 ALTER TABLE `registers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `residing_priests`
--

DROP TABLE IF EXISTS `residing_priests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `residing_priests` (
  `priest_id` varchar(15) NOT NULL,
  `priest_name` varchar(150) NOT NULL DEFAULT 'Unknown',
  `priest_status` varchar(45) NOT NULL DEFAULT 'Inactive',
  `created_by` varchar(15) NOT NULL,
  `date_created` date NOT NULL,
  `time_created` time NOT NULL,
  PRIMARY KEY (`priest_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `residing_priests`
--

LOCK TABLES `residing_priests` WRITE;
/*!40000 ALTER TABLE `residing_priests` DISABLE KEYS */;
INSERT INTO `residing_priests` VALUES ('NA','NA','Inactive','','0000-00-00','00:00:00'),('PR-1','Fr. Joseph Salando','Active','','0000-00-00','00:00:00'),('PT-3','Juan Dela Cruz','Active','prms-0000','2018-12-22','18:56:15');
/*!40000 ALTER TABLE `residing_priests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scheduling_log`
--

DROP TABLE IF EXISTS `scheduling_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `scheduling_log` (
  `log_id` varchar(15) NOT NULL,
  `log_code` varchar(45) NOT NULL,
  `log_date` date NOT NULL,
  `log_time` time NOT NULL,
  `log_creator` varchar(15) NOT NULL,
  `appointment_id` varchar(15) NOT NULL,
  PRIMARY KEY (`log_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scheduling_log`
--

LOCK TABLES `scheduling_log` WRITE;
/*!40000 ALTER TABLE `scheduling_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `scheduling_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `settings`
--

DROP TABLE IF EXISTS `settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `settings` (
  `setting_id` varchar(15) NOT NULL,
  `key_name` varchar(250) NOT NULL,
  `key_value` varchar(250) NOT NULL,
  PRIMARY KEY (`setting_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `settings`
--

LOCK TABLES `settings` WRITE;
/*!40000 ALTER TABLE `settings` DISABLE KEYS */;
INSERT INTO `settings` VALUES ('SK-1','Confirmation Stipend','252'),('SK-15','Print Fee Confirmation','201'),('SK-16','Print Fee Matrimonial','301'),('SK-17','Print Fee Burial','401'),('SK-2','Baptismal Stipend','302'),('SK-3','Matrimonial Stipend','502'),('SK-4','Burial Stipend','302'),('SK-5','Church Name','St. Raphael Parish'),('SK-6','Print Fee Baptismal','101.59');
/*!40000 ALTER TABLE `settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `timeslots`
--

DROP TABLE IF EXISTS `timeslots`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `timeslots` (
  `timeslot_id` varchar(15) NOT NULL,
  `timeslot` time DEFAULT NULL,
  `status` varchar(45) NOT NULL DEFAULT 'Inactive',
  PRIMARY KEY (`timeslot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `timeslots`
--

LOCK TABLES `timeslots` WRITE;
/*!40000 ALTER TABLE `timeslots` DISABLE KEYS */;
INSERT INTO `timeslots` VALUES ('TS-1','09:00:00','Active'),('TS-2','01:00:00','Active'),('TS-3','08:00:00','Active');
/*!40000 ALTER TABLE `timeslots` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `transactions`
--

DROP TABLE IF EXISTS `transactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transactions` (
  `transaction_id` varchar(15) NOT NULL,
  `type` varchar(45) CHARACTER SET latin1 NOT NULL,
  `status` varchar(45) NOT NULL,
  `tran_date` date NOT NULL,
  `tran_time` time NOT NULL,
  `completion_date` date DEFAULT NULL,
  `completion_time` time DEFAULT NULL,
  `placed_by` varchar(15) NOT NULL,
  `completed_by` varchar(15) DEFAULT NULL,
  `target_id` varchar(15) NOT NULL,
  `fee` float DEFAULT NULL,
  `or_number` varchar(250) DEFAULT '----',
  PRIMARY KEY (`transaction_id`),
  KEY `FK_12_idx` (`placed_by`),
  KEY `FK_13_idx` (`completed_by`),
  CONSTRAINT `FK_12` FOREIGN KEY (`placed_by`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_13` FOREIGN KEY (`completed_by`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `transactions`
--

LOCK TABLES `transactions` WRITE;
/*!40000 ALTER TABLE `transactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `transactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'pms_db'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-02-11 21:37:40
