using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using PredictionFunctions.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictionFunctions.Queries
{
    interface IPredictionQueries
    {
        Task Add(List<PredictionModel> predictions, string url);
    }

    public class PredictionQueries : IPredictionQueries
    {
        Prediction prediction = new Prediction();
        SqlConnection connection = new SqlConnection("Server=N176;Database=Predictions;Trusted_Connection=True");
        public PredictionQueries()
        {
            try
            {
                connection.OpenAsync();
            }
            catch (Exception)
            {
                connection.Close();
                throw;
            }
        }


        public async Task Add(List<PredictionModel> predictions, string url)
        {
            foreach (var item in predictions)
            {
                prediction.Id = Guid.NewGuid();
                prediction.Url = url;
                if (item.Probability > 90)
                    prediction.CanAdvertisement = true;
                else
                    prediction.CanAdvertisement = false;
                prediction.Probability = item.Probability;

                string sql = "INSERT INTO Predictions(Id,Url,CanAdvertisement) VALUES(@param1,@param2,@param3) INSERT INTO Probability(Id,PredictionId,Probability) VALUES(@param5,@param1,@param4)";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@param1", SqlDbType.UniqueIdentifier).Value = prediction.Id;
                    cmd.Parameters.Add("@param5", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                    cmd.Parameters.Add("@param2", SqlDbType.VarChar).Value = prediction.Url;
                    cmd.Parameters.Add("@param3", SqlDbType.Bit).Value = prediction.CanAdvertisement;
                    cmd.Parameters.Add("@param4", SqlDbType.Decimal).Value = prediction.Probability;
                    cmd.CommandType = CommandType.Text;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            connection.Close();
        }
    }
}
