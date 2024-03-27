namespace Bank_API_Service.IdService
{
    public class IdHelper
    {
        public static int GetNextId()
        {
            var newId = 1;

            if (File.Exists("id.txt"))
            {
                string id = File.ReadAllText("id.txt");

                if (int.TryParse(id, out int result))
                {
                    result++;
                    File.WriteAllText("id.txt.", result.ToString());
                    newId = result;
                }
            }
            else
            {
                File.WriteAllText("id.txt", newId.ToString());
            }

            return newId;
        }

        public static int GetNextTransactionId()
        {
            var newId = 1;

            if (File.Exists("t_id.txt"))
            {
                string id = File.ReadAllText("t_id.txt");

                if (int.TryParse(id, out int result))
                {
                    result++;
                    File.WriteAllText("t_id.txt.", result.ToString());
                    newId = result;
                }
            }
            else
            {
                File.WriteAllText("t_id.txt", newId.ToString());
            }

            return newId;
        }
    }
}
