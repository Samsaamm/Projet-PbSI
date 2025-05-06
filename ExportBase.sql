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
INSERT INTO `métrostation` VALUES (1,'Porte Maillot','2,282583847361549','48,87816265269652'),(2,'Argentine','2,2894354185422134','48,87566737565167'),(3,'Charles de Gaulle - Etoile','2,295811775235762','48,874994575223035'),(4,'George V','2,3007597849789168','48,87203776364103'),(5,'Franklin D. Roosevelt','2,309488192337112','48,868724887050476'),(6,'Champs-Elysées - Clemenceau','2,31446450132278','48,86765629124574'),(7,'Concorde','2,3229614457982586','48,866557992001624'),(8,'Tuileries','2,3296780947116407','48,86447783836653'),(9,'Palais Royal - Musée du Louvre','2,3364543677542504','48,86222226462601'),(10,'Louvre - Rivoli','2,3409696232852686','48,860871211759445'),(11,'Châtelet','2,3481609912345767','48,856953459837165'),(12,'Hôtel de Ville','2,3520676701390992','48,857352404237716'),(13,'Saint-Paul','2,3608852562751954','48,855187420656264'),(14,'Bastille','2,3687189610340877','48,85205429254951'),(15,'Gare de Lyon','2,373156593789204','48,845683205787566'),(16,'Reuilly - Diderot','2,3872010704093913','48,84721292574974'),(17,'Nation','2,395843988723737','48,84808428902576'),(18,'Porte de Vincennes','2,4108049967014993','48,847007730140405'),(19,'Château de Vincennes','2,4405400954061127','48,844317513365304'),(20,'Porte Dauphine','2,2776327175452704','48,871396794300246'),(21,'Victor Hugo','2,285828765912979','48,86992608146256'),(22,'Charles de Gaulle - Etoile','2,295811775235762','48,874994575223035'),(23,'Ternes','2,298113288617243','48,878227729914364'),(24,'Courcelles','2,303294362425212','48,8792652531651'),(25,'Monceau','2,3094129673747017','48,88056876917883'),(26,'Villiers','2,3158150926463765','48,88107318212763'),(27,'Rome','2,3213591738229717','48,88234581784814'),(28,'Place de Clichy','2,327958328010219','48,88366908732468'),(29,'Blanche','2,332484375434362','48,88376635352917'),(30,'Pigalle','2,3372111647011273','48,882020931119335'),(31,'Anvers','2,3441548403302903','48,88286856789594'),(32,'Barbès - Rochechouart','2,3506070793550973','48,883776088608904'),(33,'La Chapelle','2,360404169274098','48,88438640990481'),(34,'Stalingrad','2,3657743705581686','48,88432054792188'),(35,'Jaurès','2,3714389599856065','48,883023912481356'),(36,'Colonel Fabien','2,3704675747938633','48,87764980757344'),(37,'Belleville','2,3767355865572877','48,872286601164795'),(38,'Couronnes','2,3802889829271927','48,869193344184296'),(39,'Ménilmontant','2,3834303882303036','48,86639013919733'),(40,'Père Lachaise','2,3875798912544446','48,86244984398934'),(41,'Philippe Auguste','2,3904975967452877','48,85808942718759'),(42,'Alexandre Dumas','2,3947127337554774','48,85624467748922'),(43,'Avron','2,398187556659396','48,85164854264895'),(44,'Nation','2,395843988723737','48,84808428902576'),(45,'Porte de Champerret','2,2921124821588443','48,885652392185875'),(46,'Pereire','2,2976831860125824','48,88484432179189'),(47,'Wagram','2,304673087584433','48,88384638977923'),(48,'Malesherbes','2,30948784696001','48,882840021629804'),(49,'Villiers','2,3158150926463765','48,88107318212763'),(50,'Europe','2,3221859195205097','48,878753163096754'),(51,'Saint-Lazare','2,3254883906726116','48,87538131505992'),(52,'Havre-Caumartin','2,327651103418781','48,87366675099707'),(53,'Opéra','2,3310472867112395','48,871437428049184'),(54,'Quatre Septembre','2,3363187355954866','48,86965909643616'),(55,'Bourse','2,3406653707110454','48,868756796707615'),(56,'Sentier','2,347553815937787','48,867346603345666'),(57,'Réaumur - Sébastopol','2,3520507871028347','48,86638175580347'),(58,'Arts et Métiers','2,3565081436435453','48,865299611696805'),(59,'Temple','2,3615612345263557','48,866756628486336'),(60,'République','2,363302340780453','48,86751235686305'),(61,'Parmentier','2,3747477503599037','48,8652523948625'),(62,'Rue Saint-Maur','2,38050775608986','48,86411770001463'),(63,'Père Lachaise','2,3875798912544446','48,86244984398934'),(64,'Gambetta','2,3985373057045716','48,8650325635159'),(65,'Porte de Bagnolet','2,408754221189542','48,86453937198637'),(66,'Porte des Lilas','2,4070619733807948','48,876568598079956'),(67,'Saint-Fargeau','2,404498183168868','48,87184337127491'),(68,'Pelleport','2,4014967922879245','48,86846488606675'),(69,'Gambetta','2,3985373057045716','48,8650325635159'),(70,'Porte de Clignancourt','2,3446240991027905','48,897525630330136'),(71,'Simplon','2,347596055351641','48,894124442685495'),(72,'Marcadet - Poissonniers','2,349681541722476','48,89128043889656'),(73,'Château Rouge','2,3493658080048396','48,887078876519546'),(74,'Barbès - Rochechouart','2,3506070793550973','48,883776088608904'),(75,'Gare du Nord','2,356808788205408','48,87959170646212'),(76,'Gare de l\'Est','2,358064594418649','48,8761629935184'),(77,'Château d\'Eau','2,3560517414015854','48,87244691664745'),(78,'Strasbourg - Saint-Denis','2,354491616563833','48,8696235816895'),(79,'Réaumur - Sébastopol','2,3520507871028347','48,86638175580347'),(80,'Etienne Marcel','2,3489761968791076','48,8637030110762'),(81,'Les Halles','2,34612732517368','48,86250483939126'),(82,'Châtelet','2,3481609912345767','48,856953459837165'),(83,'Cité','2,3472322349319104','48,85493384815853'),(84,'Saint-Michel','2,343991799033287','48,8535940766199'),(85,'Odéon','2,340692291066523','48,85202455356673'),(86,'Saint-Germain-des-Prés','2,3339478107981932','48,853574562874215'),(87,'Saint-Sulpice','2,3306119530815415','48,8512086266185'),(88,'Saint-Placide','2,327054815066311','48,847006650641134'),(89,'Montparnasse Bienvenue','2,3239891852050025','48,84382361030622'),(90,'Vavin','2,328862854069104','48,842052322344095'),(91,'Raspail','2,3304669937198725','48,83915635857729'),(92,'Denfert-Rochereau','2,3320188352163855','48,833948692743945'),(93,'Mouton-Duvernet','2,32988764040842','48,831337584623185'),(94,'Alésia','2,327093234948452','48,82820106327088'),(95,'Porte d\'Orléans','2,3254932653821037','48,82341635019183'),(96,'Porte de Pantin','2,3921229723281474','48,888459210398885'),(97,'Ourcq','2,386652016759883','48,88691595224703'),(98,'Laumière','2,3793909893523915','48,885133847137844'),(99,'Jaurès','2,3714389599856065','48,883023912481356'),(100,'Stalingrad','2,3657743705581686','48,88432054792188'),(101,'Gare du Nord','2,356808788205408','48,87959170646212'),(102,'Gare de l\'Est','2,358064594418649','48,8761629935184'),(103,'Jacques Bonsergent','2,361023902048922','48,87062092531723'),(104,'République','2,363302340780453','48,86751235686305'),(105,'Oberkampf','2,3681558453945506','48,86477709757322'),(106,'Richard-Lenoir','2,371813630476951','48,85987693692056'),(107,'Bréguet-Sabin','2,370194668589751','48,85624405373815'),(108,'Bastille','2,3687189610340877','48,85205429254951'),(109,'Quai de la Rapée','2,365884650750405','48,84642732453155'),(110,'Gare d\'Austerlitz','2,3641773106918307','48,843405408577155'),(111,'Saint-Marcel','2,360721859717641','48,838512445735624'),(112,'Campo-Formio','2,3587419566993444','48,83554293280104'),(113,'Place d\'Italie','2,3555015914814845','48,83096571234431'),(114,'Charles de Gaulle - Etoile','2,295811775235762','48,874994575223035'),(115,'Kléber','2,2931461372486255','48,87148941198696'),(116,'Boissière','2,2900328376074364','48,86684834246921'),(117,'Trocadéro','2,287492796966441','48,86348766407144'),(118,'Passy','2,285839418881461','48,857515088672166'),(119,'Bir-Hakeim','2,289400737634599','48,85392536742779'),(120,'Dupleix','2,293663726731038','48,85041162485849'),(121,'La Motte-Picquet - Grenelle','2,2985257262366354','48,8496308034842'),(122,'Cambronne','2,3029417283376103','48,84754311124529'),(123,'Sèvres-Lecourbe','2,3095296104303924','48,84564768170248'),(124,'Pasteur','2,312914680473939','48,842528386594964'),(125,'Montparnasse Bienvenue','2,3239891852050025','48,84382361030622'),(126,'Edgar Quinet','2,3252865779432956','48,84090350074998'),(127,'Raspail','2,3304669937198725','48,83915635857729'),(128,'Denfert-Rochereau','2,3320188352163855','48,833948692743945'),(129,'Saint-Jacques','2,3371543709250067','48,83291600527084'),(130,'Glacière','2,343438236678703','48,831115946668106'),(131,'Corvisart','2,350611225926165','48,82986001236403'),(132,'Place d\'Italie','2,3555015914814845','48,83096571234431'),(133,'Nationale','2,362804171593507','48,83323527872786'),(134,'Chevaleret','2,3680812829727422','48,834963081160865'),(135,'Quai de la Gare','2,372766248948171','48,8370742696613'),(136,'Bercy','2,379463070185256','48,84017602717362'),(137,'Dugommier','2,3895997700798093','48,83903652738059'),(138,'Daumesnil','2,3961486284893696','48,83943400710489'),(139,'Bel-Air','2,400867131995258','48,84142733128531'),(140,'Picpus','2,4012745388223737','48,8451032465536'),(141,'Nation','2,395843988723737','48,84808428902576'),(142,'Porte de la Villette','2,3858690331433445','48,897802691407826'),(143,'Corentin Cariou','2,3822915571646326','48,89467265910835'),(144,'Crimée','2,376935736982379','48,89088575798087'),(145,'Riquet','2,3736694647431955','48,88815726111783'),(146,'Stalingrad','2,3657743705581686','48,88432054792188'),(147,'Louis Blanc','2,364424862493677','48,88120621087799'),(148,'Château Landon','2,362017935510607','48,87844145478439'),(149,'Gare de l\'Est','2,358064594418649','48,8761629935184'),(150,'Poissonnière','2,3487397513900725','48,87716484830221'),(151,'Cadet','2,344446309702273','48,87596328412753'),(152,'Le Peletier','2,340150559706373','48,87495940509292'),(153,'Chaussée d\'Antin - La Fayette','2,333738594408701','48,873134204826634'),(154,'Opéra','2,3310472867112395','48,871437428049184'),(155,'Pyramides','2,3346236060049224','48,86575552642865'),(156,'Palais Royal - Musée du Louvre','2,3364543677542504','48,86222226462601'),(157,'Pont Neuf','2,341776749148541','48,858546338320174'),(158,'Châtelet','2,3481609912345767','48,856953459837165'),(159,'Pont Marie (Cité des Arts)','2,3573766690444025','48,8534598594796'),(160,'Sully - Morland','2,3618531696972473','48,85127140744555'),(161,'Jussieu','2,3549316718232247','48,846197890688764'),(162,'Place Monge','2,3521548685787703','48,842666384694624'),(163,'Censier - Daubenton','2,3516265726054986','48,84022624756566'),(164,'Les Gobelins','2,3524168018982743','48,835841621396156'),(165,'Place d\'Italie','2,3555015914814845','48,83096571234431'),(166,'Tolbiac','2,357318439213191','48,826136860805335'),(167,'Maison Blanche','2,3584129461559384','48,82214950512655'),(168,'Porte d\'Italie','2,3595297331071685','48,819106595610265'),(169,'Porte de Choisy','2,3646785940494577','48,820055928199736'),(170,'Porte d\'Ivry','2,3695112543194923','48,82148903461442'),(171,'Maison Blanche','2,3584129461559384','48,82214950512655'),(172,'Louis Blanc','2,364424862493677','48,88120621087799'),(173,'Jaurès','2,3714389599856065','48,883023912481356'),(174,'Bolivar','2,374124871187541','48,880789662027055'),(175,'Buttes Chaumont','2,381569842088007','48,87849908745056'),(176,'Botzaris','2,388900951315738','48,87953499032036'),(177,'Place des Fêtes','2,3931393703604957','48,876723661025224'),(178,'Pré-Saint-Gervais','2,398580770693527','48,88015957971629'),(179,'Danube','2,393228471227491','48,88194921397263'),(180,'Botzaris','2,388900951315738','48,87953499032036'),(181,'Balard','2,2783626618091994','48,836667893882776'),(182,'Lourmel','2,2822419598550754','48,83866086271165'),(183,'Boucicaut','2,287918431124558','48,84102416004495'),(184,'Félix Faure','2,2918472203679694','48,84268433384837'),(185,'Commerce','2,2937968421928625','48,84461151142022'),(186,'La Motte-Picquet - Grenelle','2,2985257262366354','48,8496308034842'),(187,'Ecole Militaire','2,3063456838200755','48,854919659638895'),(188,'La Tour-Maubourg','2,3104735359369832','48,85772702258638'),(189,'Invalides','2,314632660444516','48,86109201043299'),(190,'Concorde','2,3229614457982586','48,866557992001624'),(191,'Madeleine','2,3258100487932767','48,87054467576818'),(192,'Opéra','2,3310472867112395','48,871437428049184'),(193,'Richelieu - Drouot','2,33859122153854','48,872135869369885'),(194,'Grands Boulevards','2,343207266405089','48,87150476881465'),(195,'Bonne Nouvelle','2,3484813657183894','48,870571298554935'),(196,'Strasbourg - Saint-Denis','2,354491616563833','48,8696235816895'),(197,'République','2,363302340780453','48,86751235686305'),(198,'Filles du Calvaire','2,3667452977427006','48,86306988250257'),(199,'Saint-Sébastien - Froissart','2,3672615397172683','48,860968144808254'),(200,'Chemin Vert','2,36809508538599','48,85708736687211'),(201,'Bastille','2,3687189610340877','48,85205429254951'),(202,'Ledru-Rollin','2,3761412233185912','48,851337765850175'),(203,'Faidherbe - Chaligny','2,3840285663831065','48,85011054318548'),(204,'Reuilly - Diderot','2,3872010704093913','48,84721292574974'),(205,'Montgallet','2,3904000136353636','48,84400507476069'),(206,'Daumesnil','2,3961486284893696','48,83943400710489'),(207,'Michel Bizot','2,4023667399880027','48,83707743008748'),(208,'Porte Dorée','2,405873912987563','48,83501737314559'),(209,'Porte de Charenton','2,4025118728500727','48,83344922193115'),(210,'Porte de Saint-Cloud','2,2570461929221497','48,8379584046712'),(211,'Exelmans','2,259800845144345','48,84258027532826'),(212,'Michel-Ange - Molitor','2,2615119169052043','48,84491122661504'),(213,'Michel-Ange - Auteuil','2,2639895253333284','48,84797535322806'),(214,'Jasmin','2,2679332916470663','48,852433417777384'),(215,'Ranelagh','2,2699481579346337','48,855503557447456'),(216,'La Muette','2,2740962446697215','48,85809196595048'),(217,'Rue de la Pompe','2,277885897959056','48,86395170793181'),(218,'Trocadéro','2,287492796966441','48,86348766407144'),(219,'Iéna','2,2938642256822703','48,86478002130934'),(220,'Alma - Marceau','2,3011043523926697','48,86464714695772'),(221,'Franklin D. Roosevelt','2,309488192337112','48,868724887050476'),(222,'Saint-Philippe du Roule','2,310137205687824','48,872155130129364'),(223,'Miromesnil','2,316010693121023','48,87344791484716'),(224,'Saint-Augustin','2,321013597808599','48,8745466119868'),(225,'Havre-Caumartin','2,327651103418781','48,87366675099707'),(226,'Chaussée d\'Antin - La Fayette','2,333738594408701','48,873134204826634'),(227,'Richelieu - Drouot','2,33859122153854','48,872135869369885'),(228,'Grands Boulevards','2,343207266405089','48,87150476881465'),(229,'Bonne Nouvelle','2,3484813657183894','48,870571298554935'),(230,'Strasbourg - Saint-Denis','2,354491616563833','48,8696235816895'),(231,'République','2,363302340780453','48,86751235686305'),(232,'Oberkampf','2,3681558453945506','48,86477709757322'),(233,'Saint-Ambroise','2,373880538211547','48,861416656961026'),(234,'Voltaire','2,3800316941488036','48,85766243343786'),(235,'Charonne','2,384782396448888','48,854939442500324'),(236,'Rue des Boulets','2,3891047050322425','48,85221407238374'),(237,'Nation','2,395843988723737','48,84808428902576'),(238,'Buzenval','2,401170769368412','48,851762438884066'),(239,'Maraîchers','2,406038544798052','48,8527310195318'),(240,'Porte de Montreuil','2,4107181540274945','48,85348263025835'),(241,'Gare d\'Austerlitz','2,3641773106918307','48,843405408577155'),(242,'Jussieu','2,3549316718232247','48,846197890688764'),(243,'Cardinal Lemoine','2,3513280016731755','48,84670003431487'),(244,'Maubert - Mutualité','2,348280413169489','48,850195465121296'),(245,'Cluny - La Sorbonne','2,3448963012120783','48,85102687592628'),(246,'Odéon','2,340692291066523','48,85202455356673'),(247,'Mabillon','2,335142819346475','48,8528437373513'),(248,'Sèvres - Babylone','2,3268596631217364','48,851565305443174'),(249,'Vaneau','2,321351242354735','48,848894569110655'),(250,'Duroc','2,316521219456521','48,84700989266375'),(251,'Ségur','2,3071378494033836','48,84716726904679'),(252,'La Motte-Picquet - Grenelle','2,2985257262366354','48,8496308034842'),(253,'Avenue Emile Zola','2,2950189971435497','48,847038012756926'),(254,'Charles Michels','2,2856210133667165','48,84660453144916'),(255,'Javel - André Citroën','2,278009496527358','48,846181105274994'),(256,'Eglise d\'Auteuil','2,269111389021891','48,84714336050894'),(257,'Michel-Ange - Auteuil','2,2639895253333284','48,84797535322806'),(258,'Porte d\'Auteuil','2,2582807445955355','48,84790412268101'),(259,'Michel-Ange - Molitor','2,2615119169052043','48,84491122661504'),(260,'Chardon Lagache','2,266905475940289','48,845088298230586'),(261,'Mirabeau','2,2730641263728324','48,84707750639776'),(262,'Châtelet','2,3481609912345767','48,856953459837165'),(263,'Hôtel de Ville','2,3520676701390992','48,857352404237716'),(264,'Rambuteau','2,3532739485947705','48,861190156002216'),(265,'Arts et Métiers','2,3565081436435453','48,865299611696805'),(266,'République','2,363302340780453','48,86751235686305'),(267,'Goncourt','2,370764434748399','48,870007016890526'),(268,'Belleville','2,3767355865572877','48,872286601164795'),(269,'Pyrénées','2,3852029155695305','48,873818789232516'),(270,'Jourdain','2,3893253789100948','48,875247347693374'),(271,'Place des FÃªtes','2,3931393703604957','48,876723661025224'),(272,'Télégraphe','2,398648089960593','48,87551038323271'),(273,'Porte des Lilas','2,4070619733807948','48,876568598079956'),(274,'Porte de la Chapelle','2,3592485423103606','48,897402421722504'),(275,'Marx Dormoy','2,359808478084023','48,890579577619285'),(276,'Marcadet - Poissonniers','2,349681541722476','48,89128043889656'),(277,'Jules Joffrin','2,3443200293804813','48,892492267254006'),(278,'Lamarck - Caulaincourt','2,338583824383193','48,88968180901888'),(279,'Abbesses','2,338394635220906','48,884392717043454'),(280,'Pigalle','2,3372111647011273','48,882020931119335'),(281,'Saint-Georges','2,3375706403796688','48,8784165939537'),(282,'Notre-Dame-de-Lorette','2,3378736192226373','48,876035030332574'),(283,'Trinité - d\'Estienne d\'Orves','2,333049172418235','48,87633650635933'),(284,'Saint-Lazare','2,3254883906726116','48,87538131505992'),(285,'Madeleine','2,3258100487932767','48,87054467576818'),(286,'Concorde','2,3229614457982586','48,866557992001624'),(287,'Assemblée Nationale','2,3205758366475546','48,86107193857794'),(288,'Solférino','2,323075964251541','48,85853194878038'),(289,'Rue du Bac','2,325699898452411','48,85588467753727'),(290,'Sèvres - Babylone','2,3268596631217364','48,851565305443174'),(291,'Rennes','2,3277871847873866','48,848332828092396'),(292,'Notre-Dame des Champs','2,3286958054987066','48,84507774468957'),(293,'Montparnasse Bienvenue','2,3239891852050025','48,84382361030622'),(294,'Falguière','2,3175512356170267','48,84431768258609'),(295,'Pasteur','2,312914680473939','48,842528386594964'),(296,'Volontaires','2,3079833805768506','48,84141172746672'),(297,'Vaugirard','2,3010745156545447','48,83943791712863'),(298,'Convention','2,2963913864809165','48,837135052856674'),(299,'Porte de Versailles','2,2877417091423338','48,83251953156517'),(300,'Porte de Vanves','2,3053323866574726','48,82761346305592'),(301,'Plaisance','2,313860489344305','48,83175036211914'),(302,'Pernety','2,3183937066147005','48,83407867882665'),(303,'Gaîté','2,322353996778806','48,83852608407424'),(304,'Montparnasse Bienvenue','2,3239891852050025','48,84382361030622'),(305,'Duroc','2,316521219456521','48,84700989266375'),(306,'Saint-François-Xavier','2,314285946180555','48,85129154999027'),(307,'Varenne','2,3151139305096557','48,856624981798376'),(308,'Invalides','2,314632660444516','48,86109201043299'),(309,'Champs-Elysées - Clemenceau','2,31446450132278','48,86765629124574'),(310,'Miromesnil','2,316010693121023','48,87344791484716'),(311,'Saint-Lazare','2,3254883906726116','48,87538131505992'),(312,'Liège','2,3268526712397546','48,87953493642774'),(313,'Place de Clichy','2,327958328010219','48,88366908732468'),(314,'La Fourche','2,3257141171661346','48,88743365578895'),(315,'Guy Môquet','2,3274832586261254','48,89300144101368'),(316,'Porte de Saint-Ouen','2,329048340744649','48,89749783637955'),(317,'La Fourche','2,3257141171661346','48,88743365578895'),(318,'Brochant','2,3199048172093653','48,8906518702421'),(319,'Porte de Clichy','2,3132083507588095','48,894431287069544'),(320,'Porte de Clichy','2,3132083507588095','48,894431287069544'),(321,'Pont Cardinet','2,3153681405051523','48,888103402695'),(322,'Saint-Lazare','2,3254883906726116','48,87538131505992'),(323,'Madeleine','2,3258100487932767','48,87054467576818'),(324,'Pyramides','2,3346236060049224','48,86575552642865'),(325,'Châtelet','2,3481609912345767','48,856953459837165'),(326,'Gare de Lyon','2,373156593789204','48,845683205787566'),(327,'Bercy','2,379463070185256','48,84017602717362'),(328,'Cour Saint-Emilion','2,386617850214057','48,83331930289509'),(329,'Bibliothèque François Mitterrand','2,3764873711683','48,82992576498057'),(330,'Olympiades','2,3669231215308066','48,82712344048104'),(331,'Olympiades','2,3669231215308066','48,82712344048104'),(332,'Maison Blanche','2,3584129461559384','48,82214950512655');
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

-- Dump completed on 2025-05-06 20:29:22

CREATE USER 'admin'@'localhost' IDENTIFIED BY 'root';
GRANT ALL PRIVILEGES ON projet_psi.* TO 'admin'@'localhost';

CREATE USER 'client'@'localhost' IDENTIFIED BY 'motdepasse_client';
GRANT SELECT, INSERT ON projet_psi.avis TO 'client'@'localhost';
GRANT SELECT, INSERT, UPDATE ON projet_psi.client TO 'client'@'localhost';
GRANT SELECT, INSERT, UPDATE ON projet_psi.commande TO 'client'@'localhost';
GRANT SELECT ON projet_psi.cuisinier TO 'client'@'localhost';
GRANT SELECT ON projet_psi.effectuer TO 'client'@'localhost';
GRANT SELECT, INSERT ON projet_psi.historiquetransactions TO 'client'@'localhost';
GRANT SELECT ON projet_psi.lignemétro TO 'client'@'localhost';
GRANT SELECT, INSERT ON projet_psi.livraison TO 'client'@'localhost';
GRANT SELECT ON projet_psi.métrostation TO 'client'@'localhost';
GRANT SELECT, UPDATE ON projet_psi.plat TO 'client'@'localhost';
GRANT SELECT, INSERT, UPDATE ON projet_psi.utilisateur TO 'client'@'localhost';

CREATE USER 'cuisinier'@'localhost' IDENTIFIED BY 'motdepasse_cuisinier';
GRANT SELECT ON projet_psi.avis TO 'cuisinier'@'localhost';
GRANT SELECT ON projet_psi.client TO 'cuisinier'@'localhost';
GRANT SELECT, UPDATE ON projet_psi.commande TO 'cuisinier'@'localhost';
GRANT SELECT, INSERT, SELECT ON projet_psi.cuisinier TO 'cuisinier'@'localhost';
GRANT SELECT, INSERT, UPDATE ON projet_psi.effectuer TO 'cuisinier'@'localhost';
GRANT SELECT, INSERT ON projet_psi.historiquetransactions TO 'cuisinier'@'localhost';
GRANT SELECT ON projet_psi.lignemétro TO 'cuisinier'@'localhost';
GRANT SELECT, INSERT, UPDATE ON projet_psi.livraison TO 'cuisinier'@'localhost';
GRANT SELECT ON projet_psi.métrostation TO 'cuisinier'@'localhost';
GRANT SELECT, INSERT, UPDATE ON projet_psi.plat TO 'cuisinier'@'localhost';
GRANT SELECT, INSERT, UPDATE ON projet_psi.utilisateur TO 'cuisinier'@'localhost';

FLUSH PRIVILEGES;
