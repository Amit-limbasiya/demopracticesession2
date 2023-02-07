using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace demopractiesession2
{
    class Program
    {
        static string connectionDetails = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\Amit Limbasiya\source\repos\demopracticesession2\demopracticesession2\Database1.mdf"";Integrated Security=True";

        static int insertedRecords = 0;
        const int totalrecords = 30_00_000;
        const int num_of_task = 15;
        const int recordsPerTask = totalrecords / num_of_task;
        const int remainingRecords = totalrecords % num_of_task;
        static string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static string lowercase = "abcdefghijklmnopqrstuvwxyz";
        static string numerics = "0123456789";
        static string special_chars = "!~`=-+<>;:][)(@#$%&*_?.";
         
        static Random random = new Random();

        public static async Task TruncateTable()
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionDetails);
                await conn.OpenAsync();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "truncate table password_store";
                await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            catch
            {
                Console.WriteLine("Exception occurs...");
            }
        }

        public static void Main(string[] args)
        {
            TruncateTable();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Passwords are inserting into database...");
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < num_of_task; i++)
            {
                Task t1=InsertRecord(recordsPerTask);
                
                tasks.Add(t1);
            }

            if (remainingRecords != 0)
            {
                Task t1 = InsertRecord(remainingRecords);
                
                tasks.Add(t1);
            }
            Task t=Task.WhenAll(tasks);
            t.Wait();
            
            stopwatch.Stop();
            double seconds = stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine("Passwords inserted...");
            Console.WriteLine("Program takes " + seconds + " seconds.");
            Console.WriteLine("Press any key...");
            Console.ReadKey();

        }
        public static int p = 0;
        public static async Task InsertRecord(int numOfRecords)
        {
            SqlConnection connection = new SqlConnection(connectionDetails);
            await connection.OpenAsync();
           
            for (int i = 0; i < numOfRecords; i++)
            {
                if (insertedRecords > totalrecords)
                {
                    break;
                }
                else
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "insert into password_store (password) values (@pw)";
                    cmd.Parameters.AddWithValue("@pw", GetPassword());
                    try
                    {

                        await cmd.ExecuteNonQueryAsync();
                        insertedRecords++;
                    }
                    catch (Exception ex)
                    { 
                       //Console.WriteLine("Skipping duplicate entries...");
                        i--;
                    }
                        
                }
            }
            await connection.CloseAsync();
        }
        static int u = 0, l = 0, n = 0, s = 0;
        static string GetPassword()
        {
            string pw = "";

            u = random.Next(1, 11);
            s = random.Next(1, 5);
            n = random.Next(1, 5);
            l = 20 - u - s - n;

            for (int i = 0; i < u; i++)
            {
                pw += uppercase[random.Next(0, uppercase.Length)].ToString();
            }
            for (int i = 0; i < l; i++)
            {
                pw += lowercase[random.Next(0, lowercase.Length)].ToString();
            }
            for (int i = 0; i < n; i++)
            {
                pw += numerics[random.Next(0, numerics.Length)].ToString();
            }
            for (int i = 0; i < s; i++)
            {
                pw += special_chars[random.Next(0, special_chars.Length)].ToString();
            }
            //string str = new string(pw.ToCharArray().OrderBy(s => (random.Next(2) % 2) == 0).ToArray());
            return pw;
        }
    }
}