using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Linq;
using System.Diagnostics;

namespace Projet
{
    internal class ExcelReader
    {
        public static void ReadExcel(string filePath){
            if(!File.Exists(filePath)){
                Console.WriteLine("Fichier introuvable !");
                return;
            }

            using (ZipArchive archive = ZipFile.OpenRead(filePath))
            {
                // Chercher le fichier contenant les données (xl/worksheets/sheet1.xml)
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.StartsWith("xl/worksheets/sheet1.xml"))
                    {
                        using (Stream stream = entry.Open())
                        {
                            ReadSheetData(stream);
                        }
                        break;
                    }
                }
            }
        }

        private static void ReadSheetData(Stream xmlStream)
        {
            using (XmlReader reader = XmlReader.Create(xmlStream))
            {
                while (reader.Read())
                {
                    // Lire chaque cellule (cellule = balise <c>)
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "c")
                    {
                        reader.MoveToAttribute("r");  // Position (ex: A1, B1, C2...)
                        string cellPosition = reader.Value;
                        
                        reader.ReadToDescendant("v"); // Lire la valeur de la cellule
                        string cellValue = reader.ReadElementContentAsString();
    
                        Console.WriteLine($"Cellule {cellPosition} : {cellValue}");
                    }
                }
            }
        }
    }
}