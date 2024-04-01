namespace Bank_API_Service.IdService
{
    public class IdHelper
    {
        public static int GetNextId(string filename)
        {
            var newId = 1;

            if (File.Exists(filename))
            {
                string id = File.ReadAllText(filename);

                if (int.TryParse(id, out int result))
                {
                    result++;
                    File.WriteAllText(filename, result.ToString());
                    newId = result;
                }
            }
            else
            {
                File.WriteAllText(filename, newId.ToString());
            }

            return newId;
        }
    }
}
