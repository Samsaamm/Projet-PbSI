using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


class Program{

    static List<string[]> ReadFile(string filename){
        StreamReader SReader = null;
        List<string[]> File = new List<string[]>();
        try{
            SReader = new StreamReader(filename);
            string line;
            while ((line = SReader.ReadLine()) != null){
                File.Add(line.Split(' '));
            }
        }catch(Exception e){
            Console.WriteLine(e.ToString());
        }finally{
            if(SReader != null){
                SReader.Close();
            }
        }

        return File;
    }

    static void Traitement(List<string[]> File){
        for(int i = 0; i < File.Count; i++){
            string[] line = File[i];
            for(int j = 0; j < line.Length; j++){
                
            }
        }
    }



    public static void Main(){
        /* List<string[]> File = new List<string[]>();
        File = ReadFile("soc-karate.mtx");
        Console.WriteLine(File[23]);    */
        Graphe Test = new Graphe("soc-karate");
    }
}
