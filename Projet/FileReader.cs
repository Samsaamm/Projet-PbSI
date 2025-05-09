using System.Xml;
using System.Linq;
using System.Diagnostics;
using OfficeOpenXml;

namespace Projet{
    public class FileReader<T>{
        public List<Noeud<T>> noeuds = new List<Noeud<T>>();
        public List<Lien<T>> liens = new List<Lien<T>>();

        /// <summary>
        /// Constructeur lisant le fichier 
        /// </summary>
        /// <param name="filepath">Chemin vers le fichier</param>
        public FileReader(string filepath){
            if(filepath.Substring(filepath.Length - 4) == ".mtx"){
                StreamReader? SReader = null;
                List<string> File = new List<string>();
                try{
                    SReader = new StreamReader(filepath);
                    string? line;
                    while((line = SReader.ReadLine()) != null){
                        if(line[0] != '%'){
                            File.Add(line);
                        }
                    }
                    for(int i = 0; i < Convert.ToInt32(File[0].Split(' ')[0]); i++){
                        noeuds.Add(new Noeud<T>(i + 1, (T)Convert.ChangeType(i + 1, typeof(T))));
                    } 
                    for(int i = 1; i < File.Count; i++){
                        string[] data = File[i].Split(' ');
                        liens.Add(new Lien<T>(noeuds[Convert.ToInt32(data[0]) - 1], noeuds[Convert.ToInt32(data[1]) - 1], 1));
                    }
                }catch(Exception e){
                    Console.Error.WriteLine(e.ToString());
                }finally{
                    if(SReader != null){
                        SReader.Close();
                    }
                }
            }else if(filepath.Substring(filepath.Length - 5) == ".xlsx"){
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(filepath))){
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;
                    string[,] data = new string[rowCount, colCount];
                    for (int i = 1; i <= rowCount; i++){
                        for (int j = 1; j <= colCount; j++){
                            data[i - 1, j - 1] = worksheet.Cells[i, j].Text;
                        }
                    }
                    for(int i = 1; i < data.GetLength(0); i++){
                        noeuds.Add(new Noeud<T>(i, (T)Convert.ChangeType(data[i,2], typeof(T)), Convert.ToDouble(data[i,3].Replace(".", ",")), Convert.ToDouble(data[i, 4].Replace(".", ","))));
                    }

                    worksheet = package.Workbook.Worksheets[2];
                    rowCount = worksheet.Dimension.Rows;
                    colCount = worksheet.Dimension.Columns;
                    object[,] datal = new object[rowCount, colCount];
                    for (int i = 1; i <= rowCount; i++){
                        for (int j = 1; j <= colCount; j++){
                            datal[i - 1, j - 1] = worksheet.Cells[i, j].Value;
                        }
                    }
                    for(int i = 1; i < rowCount; i++){
                        if(Convert.ToInt32(datal[i, 3]) != 0){
                            liens.Add(new Lien<T>(noeuds[Convert.ToInt32(datal[i, 0]) - 1], noeuds[Convert.ToInt32(datal[i, 3]) - 1], Convert.ToInt32(datal[i, 4])));
                            if(Convert.ToInt32(datal[i, 6]) == 1){
                                liens.Add(new Lien<T>(noeuds[Convert.ToInt32(datal[i, 3]) - 1], noeuds[Convert.ToInt32(datal[i, 0]) - 1], Convert.ToInt32(datal[i, 4])));
                            }
                        }
                    }
                } 
            }else{
                Console.WriteLine("Format de fichier non reconnu. Les fichier accepter sont : .mtx, .xlsx");
            }
        }

        /// <summary>
        /// propriété
        /// </summary>

        public List<Noeud<T>> Noeuds{
            get{ return noeuds;}
        }

        /// <summary>
        /// propriété
        /// </summary>

        public List<Lien<T>> Liens{
            get{ return liens;}
        }
    }
}