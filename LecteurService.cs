using System.IO;

namespace EzBulkNetDriveMapping {
    internal class LecteurService {
        public static List<Lecteur> ReadFile(string path) {
            var lines = File.ReadAllLines(path);

            var data = from l in lines.Skip(1)
                       let split = l.Split(';')
                       select new Lecteur {
                           Letter = split[0].ToUpper(),
                           Path = split[1]
                       };
            return data.ToList();
        }

        public static List<Lecteur> addEmptyRow() {
            Lecteur[] lecteurs = [new Lecteur()];
            return lecteurs.ToList();
        }
    }
}
