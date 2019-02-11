-- MySQL dump 10.13  Distrib 8.0.14, for macos10.14 (x86_64)
--
-- Host: localhost    Database: changeling_arionum_pool
-- ------------------------------------------------------
-- Server version	8.0.14

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8mb4 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Abuser`
--

DROP TABLE IF EXISTS `Abuser`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Abuser` (
  `id` int(11) unsigned NOT NULL,
  `account_id` int(11) unsigned DEFAULT NULL,
  `ip` int(10) unsigned NOT NULL DEFAULT '0',
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `count` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `Abuser_Account_idx` (`account_id`),
  CONSTRAINT `Abuser_Account` FOREIGN KEY (`account_id`) REFERENCES `account` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Account`
--

DROP TABLE IF EXISTS `Account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Account` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `wallet` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `wallet` (`wallet`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Argon`
--

DROP TABLE IF EXISTS `Argon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Argon` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `argon` varchar(150) NOT NULL,
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `Argon` (`argon`)
) ENGINE=InnoDB AUTO_INCREMENT=101 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Block`
--

DROP TABLE IF EXISTS `Block`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Block` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `uid` varchar(150) NOT NULL,
  `height` int(10) unsigned NOT NULL,
  `reward` decimal(10,0) NOT NULL,
  `winner` int(11) unsigned NOT NULL,
  `confirmations` int(11) NOT NULL DEFAULT '0',
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `Block_Account` (`winner`),
  CONSTRAINT `Block_Account` FOREIGN KEY (`winner`) REFERENCES `account` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `HashRate`
--

DROP TABLE IF EXISTS `HashRate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `HashRate` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `worker_id` int(11) unsigned NOT NULL,
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `cblocks_hashrate` int(10) NOT NULL,
  `gblocks_hashrate` int(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `HashRate_Worker` (`worker_id`),
  CONSTRAINT `HashRate_Worker` FOREIGN KEY (`worker_id`) REFERENCES `worker` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Payment`
--

DROP TABLE IF EXISTS `Payment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Payment` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Share`
--

DROP TABLE IF EXISTS `Share`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Share` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `block_height` int(11) NOT NULL,
  `account_id` int(10) unsigned NOT NULL,
  `argon` varchar(150) DEFAULT NULL,
  `nonce` varchar(45) DEFAULT NULL,
  `deadline` int(10) unsigned NOT NULL,
  `payment_id` int(11) unsigned DEFAULT NULL,
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `Share_Account_idx` (`account_id`),
  KEY `Share_Payment_idx` (`payment_id`),
  CONSTRAINT `Share_Account` FOREIGN KEY (`account_id`) REFERENCES `account` (`id`),
  CONSTRAINT `Share_Payment` FOREIGN KEY (`payment_id`) REFERENCES `payment` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Worker`
--

DROP TABLE IF EXISTS `Worker`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `Worker` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` int(11) unsigned NOT NULL,
  `ip` int(10) unsigned NOT NULL DEFAULT '0',
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `cblocks_hashrate` int(11) NOT NULL,
  `gblocks_hashrate` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `Worker` (`account_id`,`name`),
  CONSTRAINT `Worker_Account` FOREIGN KEY (`account_id`) REFERENCES `account` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping routines for database 'changeling_arionum_pool'
--
/*!50003 DROP PROCEDURE IF EXISTS `checkAbuse` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `checkAbuse`(
	IN _ip INT,
    IN _wallet NVARCHAR(150),
    IN _argon NVARCHAR(150)
)
checkAbuse:BEGIN
	DECLARE _argon_id INT;
    DECLARE _abuser_id INT;
    
    SELECT Abuser.id INTO _abuser_id 
    FROM Abuser 
		JOIN Account ON Account.wallet = _wallet AND Account.id = Abuser.account_id
	WHERE 
		Abuser.ip = _ip AND
        Abuser.count >= 3 AND
		TIMESTAMPDIFF(SECOND, Abuser.`timestamp`, NOW()) < 60
	LIMIT 1;
        
	IF(_abuser_id IS NOT NULL) THEN
		SELECT 1; -- limited to 3 invalid argons per minute
        LEAVE checkAbuse;
    END IF;
	
    SELECT id INTO _argon_id FROM Argon WHERE `argon` = _argon;
    
    IF(_argon_id IS NOT NULL) THEN
		CALL updateAbuse(_ip, _wallet);
        SELECT 2; -- duplicate argon detected
        LEAVE checkAbuse;
    END IF;
    
    INSERT INTO Argon(argon) VALUES(_argon);
    
	SELECT 0;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `saveBlock` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `saveBlock`(
	IN _uid VARCHAR(150),
    IN _height INT,
    IN _reward DECIMAL(10,0),
    IN _winner VARCHAR(150)
)
BEGIN
	DECLARE _account_id INT;
    
    SELECT id INTO _account_id FROM Account WHERE `wallet` = _winner LIMIT 1;
    IF _account_id IS NULL THEN
		INSERT INTO Account(`wallet`) VALUES(_winner);
        SELECT LAST_INSERT_ID() INTO _account_id;
    END IF;
    
    INSERT INTO `Block`(uid, height, reward, winner) VALUES(_uid, _height, _reward, _account_id);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `saveHashRateEntry` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `saveHashRateEntry`(
	IN _ip INT,
    IN _wallet VARCHAR(150), 
	IN _worker VARCHAR(50), 
    IN _hash_rate_cblocks INT, 
    IN _hash_rate_gblocks INT
)
BEGIN
	DECLARE _account_id INT;
	DECLARE _worker_id INT;

	START TRANSACTION;
	
    SELECT id INTO _account_id FROM Account WHERE `wallet` = _wallet LIMIT 1;
    IF _account_id IS NULL THEN
		INSERT INTO Account(`wallet`) VALUES(_wallet);
        SELECT LAST_INSERT_ID() INTO _account_id;
    END IF;
    
    SELECT id INTO _worker_id FROM Worker WHERE `name` = _worker AND `account_id` = _account_id LIMIT 1;
    IF _worker_id IS NULL THEN
		INSERT INTO Worker(`account_id`, `ip`, `name`, `cblocks_hashrate`, `gblocks_hashrate`)
			VALUES(_account_id, _ip, _worker, _hash_rate_cblocks, _hash_rate_gblocks);
		SELECT LAST_INSERT_ID() INTO _worker_id;
	ELSE
		UPDATE Worker SET `ip` = _ip, `cblocks_hashrate` = _hash_rate_cblocks, `gblocks_hashrate` = _hash_rate_gblocks, `timestamp` = CURRENT_TIMESTAMP()
			WHERE `id` = _worker_id AND `account_id` = _account_id;
    END IF;
    
    INSERT INTO HashRate(`worker_id`, `cblocks_hashrate`, `gblocks_hashrate`) 
		VALUES(_worker_id, _hash_rate_cblocks, _hash_rate_gblocks);
        
	COMMIT;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `saveShare` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `saveShare`(
	IN _wallet VARCHAR(150),
    IN _nonce VARCHAR(150),
    IN _argon VARCHAR(150),
    IN _block_height INT,
    IN _deadline INT
)
BEGIN
	DECLARE _account_id INT;
    
    SELECT id INTO _account_id FROM Account WHERE `wallet` = _winner LIMIT 1;
    IF _account_id IS NULL THEN
		INSERT INTO Account(`wallet`) VALUES(_winner);
        SELECT LAST_INSERT_ID() INTO _account_id;
    END IF;

	INSERT INTO `Share`(block_height, account_id, argon, nonce, deadline) VALUES(_block_height, _account_id, _argon, _nonce, _deadline);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `updateAbuse` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `updateAbuse`(
	IN _ip INT,
    IN _wallet NVARCHAR(150)
)
BEGIN
	DECLARE _account_id INT;
    DECLARE _abuser_id INT;
    DECLARE _timestamp DATETIME;
    DECLARE _count INT;
    
	SELECT `id` INTO _account_id FROM Account WHERE `wallet` = _wallet;
    
    IF(_account_id IS NULL) THEN
		INSERT INTO Account(`wallet`) VALUES(_wallet);
        SELECT LAST_INSERT_ID() INTO _account_id;
    END IF;
    
    SELECT `id`, `timestap`, `count` INTO _abuser_id, _timestamp, _count 
    FROM Abuser 
	WHERE 
		ip = _ip AND account_id = _account_id;
        
	IF(_abuser_id IS NULL) THEN
		INSERT INTO Abuser(account_id, ip, count) VALUES(_account_id, _ip, 1);
	ELSE
		IF(TIMESTAMPDIFF(SECOND, _timestamp, NOW()) >= 60) THEN
			SET _count = 0;
		END IF;
        
        UPDATE Abuser SET `timestamp` = CURRENT_TIMESTAMP(), `count` = (_count + 1) 
			WHERE id = _abuser_id;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-02-11 22:25:05
