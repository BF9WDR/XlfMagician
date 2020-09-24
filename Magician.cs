using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace XlfMagician
{
    public class Magician
    {
        private int StrictChangeCount { get; set; } = 0;

        private int LooseChangeCount { get; set; } = 0;
        public string SourceXlfPath{ get; set; }
        public string TargetXlfPath { get; set; }

        public (int strictChangeCount, int looseChangeCount, string resultFilePath) XlfMerge(bool isStrict)
        {
            XmlDocument sourceDoc = new XmlDocument(new NameTable());
            sourceDoc.Load(SourceXlfPath);
            //ignoring all the outer xml objects
            var sourceBody = sourceDoc.ChildNodes[1].FirstChild.FirstChild.FirstChild;
            Console.WriteLine("source parsed");


            XmlDocument targetDoc = new XmlDocument(new NameTable());
            targetDoc.Load(TargetXlfPath);
            //ignoring all the outer xml objects
            var targetBody = targetDoc.ChildNodes[1].FirstChild.FirstChild.FirstChild;
            Console.WriteLine("target parsed");
            

            foreach(XmlNode sourceElement in sourceBody.ChildNodes)
            {
                foreach (XmlNode targetElement in targetBody.ChildNodes)
                {
                    
                    if (
                        //compares source texts
                        sourceElement.ChildNodes[1].FirstChild.InnerText == targetElement.ChildNodes[1].FirstChild.InnerText                     
                    )
                    {
                        if (
                        //compares notes' inner texts (eg. Table Main Project Header - Property Caption)
                        sourceElement.ChildNodes[7].FirstChild.InnerText == targetElement.ChildNodes[7].FirstChild.InnerText
                        )
                        {
                            Console.WriteLine("match found, both sources are: " + (sourceElement.ChildNodes[1].FirstChild.InnerText));
                            Console.WriteLine(
                                "original target is: "
                                + sourceElement.ChildNodes[3].FirstChild.InnerText
                                + "\ntarget's target is: "
                                + targetElement.ChildNodes[3].FirstChild.InnerText
                            );
                            //changing the translation in the target node to the one found on source node
                            targetElement.ChildNodes[3].FirstChild.InnerText = sourceElement.ChildNodes[3].FirstChild.InnerText;
                            Console.WriteLine(
                                "target node overwritten in memory represented object" + "\n###########################"
                            );
                            //incrementing strict change counter
                            StrictChangeCount++;
                        } else
                        {
                            Console.WriteLine(
                                "match found, both sources are: " 
                                + (sourceElement.ChildNodes[1].FirstChild.InnerText) 
                                + ", but note node did not match!"
                            );
                            Console.WriteLine(
                                "original target is: "
                                + sourceElement.ChildNodes[3].FirstChild.InnerText
                                + "\ntarget's target is: "
                                + targetElement.ChildNodes[3].FirstChild.InnerText
                            );
                            if (targetElement.ChildNodes[3].FirstChild.InnerText.Contains("NAB: ")) {
                                if (isStrict)
                                {
                                    Console.WriteLine(
                                        "strict mode, target node left alone" + "\n###########################"
                                    );
                                } else
                                {
                                    //if it is not strict, change the node just as if it was matching with the note as well
                                    targetElement.ChildNodes[3].FirstChild.InnerText = sourceElement.ChildNodes[3].FirstChild.InnerText;                                 
                                    Console.WriteLine(
                                        "loose matching mode, target node overwritten in memory represented object" + "\n###########################"
                                    );
                                    //incrementing strict change counter
                                    LooseChangeCount++;
                                }
                            } else
                            {
                                Console.WriteLine(
                                        "target node already has a translation, target node left alone" + "\n###########################"
                                    );
                            }
                            
                        }
                    }
                }
            }

            //writes the file to disk
            targetDoc.Save("magicked_destination.xlf");
            Console.WriteLine("\n\n" 
                + "Modified xlf file serialized succesfully!"
            );

            return (
                StrictChangeCount,
                LooseChangeCount,
                Path.Combine(Directory.GetCurrentDirectory() , "magicked_destination.xlf")
            );
        }
    }
}
