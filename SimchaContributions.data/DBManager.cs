
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaContributions.data
{
    public class DBManager
    {
        private string _connectionString;
        private List<Contributer> Contributers;

        public DBManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Simcha> GetSimchas() { 
        
            int totalNumberofContributers = GetNumberOfContributers();
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();


            cmd.CommandText = @"SELECT * FROM Simchos ";

            connection.Open();
            List<Simcha> simchos = new List<Simcha>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                simchos.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"],
                    NumberOfContributers = $"{GetNumberOfContributersPerSimcha((int)reader["Id"])}/{totalNumberofContributers}",
                    Total=GetTotalForSimcha((int)reader["Id"]),
                    Amount = GetAmountPerSimcha((int)reader["Id"])

                }) ;

            }
            return simchos;

        }
        public int GetNumberOfContributers()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"select count (*) from Contributer";


            connection.Open();
            int x = (int) cmd.ExecuteScalar();
           return x ;


        }
        public int GetNumberOfContributersPerSimcha(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"select count(*) from [Simchos-Contributer] sc
                               where SimchaId =@id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            int x = (int)cmd.ExecuteScalar();
           return x;


        }

        public decimal GetAmountPerSimcha(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"select SUM(Amount) from [Simchos-Contributer]
                             where SimchaId=@id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            var x = cmd.ExecuteScalar();
            if (x == DBNull.Value)
            {
                return 0;
            }

            return -(decimal)x;


        }

        public List<Contributer> GetAllContributers()
        {

           
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();


            cmd.CommandText = @"SELECT * FROM Contributer ";

            connection.Open();
            List<Contributer> contributers = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributers.Add(new Contributer
                {
                    Id = (int)reader["Id"],
                    Cell=(int)reader["cell"],
                    CreatedDate = (DateTime)reader["CreatedDate"],
                    FirstName = (string)reader["FirstName"],
                    LastName=(string)reader["LastName"],
                    Balance= GetPersonsBalance((int)reader["Id"])
                    
                });
             

            }
            Contributers = contributers;

            return contributers;

        }

        public decimal GetPersonsBalance(int id)
        {
            return GetAllDepositsById(id) + GetAllContributionsById(id);
        }
        public decimal GetAllDepositsById(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = @"Select SUM(d.DepositAmount) from Contributer c
                            Left Join Deposits d
                            ON c.Id=d.ContributerId
                            where id=@id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            
            return (decimal)cmd.ExecuteScalar();


        }
        public decimal GetAllContributionsById(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"Select SUM(sc.Amount) from Contributer c
                            Left JOIN [Simchos-Contributer] sc
                            ON c.Id= sc.ContributerId
                            where id=@id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            var x = cmd.ExecuteScalar();
            if(x==DBNull.Value)
             {
                return 0;
            }
            
            return -(decimal)x;


        }
        public decimal GetTotal()
        {
            decimal sum = 0;
            sum = Contributers.Sum(sum => sum.Balance);
            return sum;
        }

        public List<HistoryItem> GetHistoryItems(int ContributerId)
        {
           
            List<HistoryItem> historyItems = new();
            historyItems.AddRange(GetContributions(ContributerId));
            historyItems.AddRange(GetDeposits(ContributerId));


            return historyItems.OrderBy(o=>o.Date).ToList();
            
        }
        private List<HistoryItem> GetContributions(int ContributerId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();



            cmd.CommandText = @"select sc.Amount, sc.Date, s.SimchaName from [Simchos-Contributer] sc
                                Join Simchos s
                                On s.Id=sc.SimchaId
                                where ContributerId=@id ";
            cmd.Parameters.AddWithValue("@id", ContributerId);


            connection.Open();
            List<HistoryItem> historyItems = new();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                historyItems.Add(new HistoryItem
                {

                    Date = (DateTime)reader["Date"],
                    Transaction = (string)reader["SimchaName"],
                    Amount = -((decimal)reader["Amount"])

                });

            }
            return historyItems;

        }
        private List<HistoryItem> GetDeposits(int ContributerId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();


            cmd.CommandText = @"select * from Deposits
                        where ContributerId=@id ";
            cmd.Parameters.AddWithValue("@id", ContributerId);

            connection.Open();
            List<HistoryItem> historyItems = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                historyItems.Add(new HistoryItem
                {

                    Date = (DateTime)reader["Date"],
                    Transaction = "Deposit",
                    Amount = (decimal)reader["DepositAmount"]

                });

            }
            return historyItems;
        }
        public void AddContributer(Contributer contributer, int initialDeposit)
        {
            
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributer (firstName, lastName, Cell, CreatedDate) " +
                "VALUES (@fname, @lname, @cell, @date) SELECT SCOPE_IDENTITY()";

            cmd.Parameters.AddWithValue("@fname", contributer.FirstName);
            cmd.Parameters.AddWithValue("@lname", contributer.LastName);
            cmd.Parameters.AddWithValue("@cell", contributer.Cell);
            cmd.Parameters.AddWithValue("@date", contributer.CreatedDate);
            connection.Open();
           int id=(int) (decimal)cmd.ExecuteScalar();
           AddDeposit(id,initialDeposit,contributer.CreatedDate);
           
        }
        public void AddDeposit(int contribid, decimal amount, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO deposits (Contributerid,DEPOSITamount, Date) " +
                "VALUES (@id, @amount, @date)";

            cmd.Parameters.AddWithValue("@id", contribid);
            cmd.Parameters.AddWithValue("@amount", amount);
          
            cmd.Parameters.AddWithValue("@date", date);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void AddSimcha(Simcha simcha)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO simchos (simchaName, Date) " +
                "VALUES (@simchaname,  @date)";

            cmd.Parameters.AddWithValue("@simchaname", simcha.Name);
            cmd.Parameters.AddWithValue("@date", simcha.Date);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Contributer> GetContrirutionsforaSimcha(int simchaid)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @" Select c.FirstName, c.LastName, c.Id, sc.Amount, sc.SimchaId from Contributer c
                                 Left JOIN [Simchos-Contributer] sc
                                 ON c.Id= sc.ContributerId
                                 where sc.SimchaId=@id";
            cmd.Parameters.AddWithValue("@id", simchaid);

            connection.Open();

            List<Contributer> contributers = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributers.Add(new Contributer
                {

                    Id = (int)reader["Id"],
                   
                   AmountPerSimcha= (decimal)reader["Amount"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Balance = GetPersonsBalance((int)reader["Id"])

                });

            }


            List<Contributer> allContributers=GetAllContributers();
            foreach (Contributer c in allContributers)
            {

                if (!contributers.Select(s => s.Id).Contains(c.Id))
                {
                    contributers.Add(c);
                }
            }

            return contributers;

             }
        public Simcha GetSimchById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "Select * from simchos where id=@id ";
               

            cmd.Parameters.AddWithValue("@id", id);
        
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            Simcha s = new();
            s.Id = (int)reader["Id"];
            s.Name = (string)reader["SimchaName"];
            s.Date = (DateTime)reader["Date"];
            // s.NumberOfContributers = $"{GetNumberOfContributersPerSimcha((int)reader["Id"])}/{totalNumberofContributers}";
            s.Amount = GetAmountPerSimcha((int)reader["Id"]);
                return s;
        }

        public void UpdateContributions(List<Contributer> contributers, int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            List<Contributer> contributersContributating = new();
                //contributers.Where(w => w.AmountPerSimcha != 0).ToList();
            foreach (Contributer c in contributers)
            {
                if(c.AmountPerSimcha != 0)
                {
                    contributersContributating.Add(c);
                }
            }
            if (contributersContributating.Count() == 0)
            {
                return;
            }
            //cmd.CommandText = @" update [Simchos-Contributer] set amount=10
            //            where SimchaId = 1 and ContributerId = 1";
            DeleteContributions(simchaId);
            cmd.CommandText = @"
                                Insert into [Simchos-Contributer] (ContributerId,SimchaId,Amount,Date)
                               values (@ContributerId, @simchaid, @amount, GETDATE())";

            foreach (Contributer c in contributersContributating) {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@simchaid", simchaId);
                cmd.Parameters.AddWithValue("@ContributerId", c.Id);
                cmd.Parameters.AddWithValue("@amount", c.Id);

            }
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void DeleteContributions( int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            List<Contributer> contributersContributating = new();
            //contributers.Where(w => w.AmountPerSimcha != 0).ToList();
            
            //cmd.CommandText = @" update [Simchos-Contributer] set amount=10
            //            where SimchaId = 1 and ContributerId = 1";
            cmd.CommandText = @"delete from [Simchos-Contributer] where simchaId = @simchaid
                               ";

       
                cmd.Parameters.AddWithValue("@simchaid", simchaId);
             

            
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public decimal GetTotalForSimcha(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"
                             select ISNUll (SUM(Amount),0) from [Simchos-Contributer]
                             where SimchaId=@id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();

            return (decimal)cmd.ExecuteScalar();


        }

    }
}
