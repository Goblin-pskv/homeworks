using System.Diagnostics;


class Program
{
    static void Main()
    {
        string folderPath = @"C:\projects\C#\homework_parallel\txt_files";
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = CountSpacesInFolderParallel(folderPath);
            
            stopwatch.Stop();
            
            Console.WriteLine($"Результат подсчета пробелов:");
            foreach (var item in result)
            {
                Console.WriteLine($"Файл: {Path.GetFileName(item.Key)}, Пробелов: {item.Value}");
            }
            Console.WriteLine($"Общее количество пробелов: {result.Values.Sum()}");
            Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static Dictionary<string, int> CountSpacesInFolderParallel(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Папка не найдена: {folderPath}");

        string[] files = Directory.GetFiles(folderPath);
        
        if (files.Length == 0)
            return new Dictionary<string, int>();

        // Создаем задачи для параллельного выполнения
        var tasks = files.Select(filePath => Task.Run(() => CountSpacesInFile(filePath)));
        
        // Ожидаем выполнения всех задач
        Task.WaitAll(tasks.ToArray());
        
        // Собираем результаты
        var results = tasks.Select(t => t.Result);
        
        return results.ToDictionary(
            result => result.FilePath,
            result => result.SpaceCount
        );
    }

    static (string FilePath, int SpaceCount) CountSpacesInFile(string filePath)
    {
        try
        {
            string content = File.ReadAllText(filePath);
            int spaceCount = content.Count(c => c == ' ');
            return (filePath, spaceCount);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла {filePath}: {ex.Message}");
            return (filePath, 0);
        }
    }
}