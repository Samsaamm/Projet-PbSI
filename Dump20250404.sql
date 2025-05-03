CREATE DATABASE  IF NOT EXISTS `projet_psi` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `projet_psi`;
-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: projet_psi
-- ------------------------------------------------------
-- Server version	8.4.4

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `avis`
--

DROP TABLE IF EXISTS `avis`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `avis` (
  `Id_avis` varchar(50) NOT NULL,
  `Id_client` varchar(50) DEFAULT NULL,
  `Id_cuisinier` varchar(50) DEFAULT NULL,
  `Note` int NOT NULL,
  `Commentaire` varchar(200) DEFAULT NULL,
  `Date_avis` date NOT NULL,
  `Id_client_1` varchar(50) NOT NULL,
  `Id_cuisinier_1` varchar(50) NOT NULL,
  PRIMARY KEY (`Id_avis`),
  KEY `Id_client_1` (`Id_client_1`),
  KEY `Id_cuisinier_1` (`Id_cuisinier_1`),
  CONSTRAINT `avis_ibfk_1` FOREIGN KEY (`Id_client_1`) REFERENCES `client` (`Id_client`),
  CONSTRAINT `avis_ibfk_2` FOREIGN KEY (`Id_cuisinier_1`) REFERENCES `cuisinier` (`Id_cuisinier`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `avis`
--

LOCK TABLES `avis` WRITE;
/*!40000 ALTER TABLE `avis` DISABLE KEYS */;
/*!40000 ALTER TABLE `avis` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `client`
--

DROP TABLE IF EXISTS `client`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `client` (
  `Id_client` varchar(50) NOT NULL,
  `Id_station` int NOT NULL,
  `Id_utilisateur` varchar(50) NOT NULL,
  PRIMARY KEY (`Id_client`),
  UNIQUE KEY `Id_utilisateur` (`Id_utilisateur`),
  KEY `Id_station` (`Id_station`),
  CONSTRAINT `client_ibfk_1` FOREIGN KEY (`Id_station`) REFERENCES `métrostation` (`Id_station`),
  CONSTRAINT `client_ibfk_2` FOREIGN KEY (`Id_utilisateur`) REFERENCES `utilisateur` (`Id_utilisateur`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `client`
--

LOCK TABLES `client` WRITE;
/*!40000 ALTER TABLE `client` DISABLE KEYS */;
/*!40000 ALTER TABLE `client` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `commande`
--

DROP TABLE IF EXISTS `commande`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `commande` (
  `Id_commande` varchar(50) NOT NULL,
  `Id_cuisinier` varchar(50) NOT NULL,
  `Id_client` varchar(50) NOT NULL,
  `Id_plat` varchar(50) NOT NULL,
  `Quantité` int NOT NULL,
  `Date_commande` date NOT NULL,
  `Prix` decimal(15,2) NOT NULL,
  `Statut` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id_commande`),
  KEY `Id_plat` (`Id_plat`),
  KEY `Id_client` (`Id_client`),
  KEY `Id_cuisinier` (`Id_cuisinier`),
  CONSTRAINT `commande_ibfk_1` FOREIGN KEY (`Id_plat`) REFERENCES `plat` (`Id_plat`),
  CONSTRAINT `commande_ibfk_2` FOREIGN KEY (`Id_client`) REFERENCES `client` (`Id_client`),
  CONSTRAINT `commande_ibfk_3` FOREIGN KEY (`Id_cuisinier`) REFERENCES `cuisinier` (`Id_cuisinier`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `commande`
--

LOCK TABLES `commande` WRITE;
/*!40000 ALTER TABLE `commande` DISABLE KEYS */;
/*!40000 ALTER TABLE `commande` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cuisinier`
--

DROP TABLE IF EXISTS `cuisinier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuisinier` (
  `Id_cuisinier` varchar(50) NOT NULL,
  `Id_station` int NOT NULL,
  `Id_utilisateur` varchar(50) NOT NULL,
  PRIMARY KEY (`Id_cuisinier`),
  UNIQUE KEY `Id_utilisateur` (`Id_utilisateur`),
  KEY `Id_station` (`Id_station`),
  CONSTRAINT `cuisinier_ibfk_1` FOREIGN KEY (`Id_station`) REFERENCES `métrostation` (`Id_station`),
  CONSTRAINT `cuisinier_ibfk_2` FOREIGN KEY (`Id_utilisateur`) REFERENCES `utilisateur` (`Id_utilisateur`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cuisinier`
--

LOCK TABLES `cuisinier` WRITE;
/*!40000 ALTER TABLE `cuisinier` DISABLE KEYS */;
/*!40000 ALTER TABLE `cuisinier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `effectuer`
--

DROP TABLE IF EXISTS `effectuer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `effectuer` (
  `Id_cuisinier` varchar(50) NOT NULL,
  `Id_livraison` varchar(50) NOT NULL,
  PRIMARY KEY (`Id_cuisinier`,`Id_livraison`),
  KEY `Id_livraison` (`Id_livraison`),
  CONSTRAINT `effectuer_ibfk_1` FOREIGN KEY (`Id_cuisinier`) REFERENCES `cuisinier` (`Id_cuisinier`),
  CONSTRAINT `effectuer_ibfk_2` FOREIGN KEY (`Id_livraison`) REFERENCES `livraison` (`Id_livraison`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `effectuer`
--

LOCK TABLES `effectuer` WRITE;
/*!40000 ALTER TABLE `effectuer` DISABLE KEYS */;
/*!40000 ALTER TABLE `effectuer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `historiquetransactions`
--

DROP TABLE IF EXISTS `historiquetransactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `historiquetransactions` (
  `Id_transaction` varchar(50) NOT NULL,
  `Id_cuisinier` varchar(50) DEFAULT NULL,
  `Id_client` varchar(50) DEFAULT NULL,
  `Montant` decimal(15,2) NOT NULL,
  `Date_transaction` date NOT NULL,
  `Id_cuisinier_1` varchar(50) NOT NULL,
  `Id_client_1` varchar(50) NOT NULL,
  PRIMARY KEY (`Id_transaction`),
  UNIQUE KEY `Id_cuisinier_1` (`Id_cuisinier_1`),
  UNIQUE KEY `Id_client_1` (`Id_client_1`),
  CONSTRAINT `historiquetransactions_ibfk_1` FOREIGN KEY (`Id_cuisinier_1`) REFERENCES `cuisinier` (`Id_cuisinier`),
  CONSTRAINT `historiquetransactions_ibfk_2` FOREIGN KEY (`Id_client_1`) REFERENCES `client` (`Id_client`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `historiquetransactions`
--

LOCK TABLES `historiquetransactions` WRITE;
/*!40000 ALTER TABLE `historiquetransactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `historiquetransactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lignemétro`
--

DROP TABLE IF EXISTS `lignemétro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lignemétro` (
  `Id_ligne` varchar(50) NOT NULL,
  `Num_ligne` int NOT NULL,
  `Liste_stations` varchar(2000) NOT NULL,
  PRIMARY KEY (`Id_ligne`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lignemétro`
--

LOCK TABLES `lignemétro` WRITE;
/*!40000 ALTER TABLE `lignemétro` DISABLE KEYS */;
/*!40000 ALTER TABLE `lignemétro` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `livraison`
--

DROP TABLE IF EXISTS `livraison`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `livraison` (
  `Id_livraison` varchar(50) NOT NULL,
  `Id_cuisinier` varchar(50) DEFAULT NULL,
  `Id_commande` varchar(50) DEFAULT NULL,
  `Adresse_départ` varchar(50) NOT NULL,
  `Adresse_arrivée` varchar(50) NOT NULL,
  `Date_heure_arrivée` datetime NOT NULL,
  `Distance` decimal(15,2) NOT NULL,
  `Id_commande_1` varchar(50) NOT NULL,
  PRIMARY KEY (`Id_livraison`),
  UNIQUE KEY `Id_commande_1` (`Id_commande_1`),
  CONSTRAINT `livraison_ibfk_1` FOREIGN KEY (`Id_commande_1`) REFERENCES `commande` (`Id_commande`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `livraison`
--

LOCK TABLES `livraison` WRITE;
/*!40000 ALTER TABLE `livraison` DISABLE KEYS */;
/*!40000 ALTER TABLE `livraison` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `métrostation`
--

DROP TABLE IF EXISTS `métrostation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `métrostation` (
  `Id_station` int NOT NULL,
  `Nom_station` varchar(50) NOT NULL,
  `Latitude` varchar(50) NOT NULL,
  `Longitude` varchar(50) NOT NULL,
  PRIMARY KEY (`Id_station`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `métrostation`
--

LOCK TABLES `métrostation` WRITE;
/*!40000 ALTER TABLE `métrostation` DISABLE KEYS */;
/*!40000 ALTER TABLE `métrostation` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plat`
--

DROP TABLE IF EXISTS `plat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `plat` (
  `Id_plat` varchar(50) NOT NULL,
  `Id_cuisinier` varchar(50) DEFAULT NULL,
  `Nom` varchar(50) NOT NULL,
  `Type` varchar(50) NOT NULL,
  `Nombre_pers` int NOT NULL,
  `Date_fabrication` date NOT NULL,
  `Date_peremption` date NOT NULL,
  `Prix` decimal(15,2) NOT NULL,
  `Nationalité` varchar(50) NOT NULL,
  `Régime` varchar(50) NOT NULL,
  `Ingrédients` varchar(100) NOT NULL,
  PRIMARY KEY (`Id_plat`),
  KEY `Id_cuisinier` (`Id_cuisinier`),
  CONSTRAINT `plat_ibfk_1` FOREIGN KEY (`Id_cuisinier`) REFERENCES `cuisinier` (`Id_cuisinier`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plat`
--

LOCK TABLES `plat` WRITE;
/*!40000 ALTER TABLE `plat` DISABLE KEYS */;
/*!40000 ALTER TABLE `plat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `utilisateur`
--

DROP TABLE IF EXISTS `utilisateur`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `utilisateur` (
  `Id_utilisateur` varchar(50) NOT NULL,
  `Nom` varchar(50) DEFAULT NULL,
  `Prénom` varchar(50) NOT NULL,
  `Addresse` varchar(70) NOT NULL,
  `Téléphone` int NOT NULL,
  `Email` varchar(70) NOT NULL,
  `Mot_de_passe` varchar(50) NOT NULL,
  PRIMARY KEY (`Id_utilisateur`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `utilisateur`
--

LOCK TABLES `utilisateur` WRITE;
/*!40000 ALTER TABLE `utilisateur` DISABLE KEYS */;
/*!40000 ALTER TABLE `utilisateur` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-04 16:04:10
