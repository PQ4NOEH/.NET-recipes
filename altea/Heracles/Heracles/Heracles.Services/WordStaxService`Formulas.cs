namespace Heracles.Services
{
    using System.Collections.Generic;
    using System.Data.SqlClient;

    using Altea.Classes.Stax;
    using Altea.Classes.Stax.WordStax;
    using Altea.Contracts;

    public abstract partial class WordStaxService : Service<IStaxChannel>
    {
        private static IStackFormula WordStack(SqlDataReader reader, int numberData, int extraData)
        {
            reader.Read();

            WordStackFormula formula = new WordStackFormula
                {
                    Id = (int)reader["id"],
                    Data = (string)reader["data"],
                    Answer = (string)reader["answer"]
                };

            string[] otherData = new string[extraData];

            int i = 0;
            while (reader.Read())
            {
                otherData[i++] = (string)reader["answer"];
            }

            formula.OtherData = otherData;

            return formula;
        }

        private static IStackFormula AudioWordStack(SqlDataReader reader, int numberData, int extraData)
        {
            reader.Read();

            AudioWordStackFormula formula = new AudioWordStackFormula
            {
                Id = (int)reader["id"],
                Data = (string)reader["data"]
            };

            string[] otherData = new string[extraData];

            int i = 0;
            while (reader.Read())
            {
                otherData[i++] = (string)reader["data"];
            }

            formula.OtherData = otherData;

            return formula;
        }

        private static IStackFormula ExtendedWordStack(SqlDataReader reader, int numberData, int extraData)
        {
            ExtendedWordStackFormula formula = new ExtendedWordStackFormula
            {
                Data = new WordStackFormula[numberData],
                OtherData = new string[extraData]
            };

            int i = -1;
            int j = 0;
            int lastId = -1;

            while (reader.Read())
            {
                int? id = reader["id"] as int?;

                if (id.HasValue)
                {
                    WordStackFormula subFormula;

                    if (id.Value != lastId)
                    {
                        lastId = id.Value;

                        subFormula = new WordStackFormula
                            {
                                Id = lastId,
                                Data = (string)reader["data"],
                                Answer = null,
                                OtherData = new List<string>()
                            };

                        ((WordStackFormula[])formula.Data)[++i] = subFormula;
                    }

                    subFormula = ((WordStackFormula[])formula.Data)[i];
                    ((List<string>)subFormula.OtherData).Add((string)reader["answer"]);
                }
                else
                {
                    ((string[])formula.OtherData)[j++] = (string)reader["answer"];
                }
            }

            return formula;
        }

        private static IStackFormula SentenceWordStack(SqlDataReader reader, int numberData, int extraData)
        {
            SentenceWordStackFormula formula = new SentenceWordStackFormula
            {
                Ids = new int[numberData],
                Datas = new string[numberData],
                Sentences = new string[numberData]
            };

            int i = 0;
            while (reader.Read())
            {
                ((int[])formula.Ids)[i] = (int)reader["id"];
                ((string[])formula.Datas)[i] = (string)reader["data"];
                ((string[])formula.Sentences)[i] = (string)reader["answer"];

                i++;
            }

            return formula;
        }
    }
}
