//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AvaloniaEidos.ViewModels;
//using CommunityToolkit.Mvvm.ComponentModel;

//namespace AvaloniaEidos.Models
//{
//    public class TaskModel
//    {
//        private Task task;
//        private bool isTaskRunning = false;
//        private int newId = 0;
//        public int Id { get; private set; }
//        public string Type { get; set; }
//        public bool IsCompleted { get; set; }
//        public bool IsSuccess { get; set; }
//        public int CurrentProgress { get; set; }


//        private readonly Random random;
//        public event EventHandler<double> ProgressChanged;
//        public event EventHandler<bool> TaskCompleted;
//        public TaskModel(string type)
//        {
//            this.Id = newId++;
//            this.Type = type;
//            this.IsCompleted = false;
//            this.CurrentProgress = 0;

//            random = new Random();
//        }
//        public void StartTask()
//        {
//                UpdateProgress();                

//        }
//        private void UpdateProgress()
//        {
//            try
//            {
//                Task.Run(async () =>
//                {
//                    CurrentProgress += random.Next(1, 30);
//                    while (CurrentProgress < 100)
//                    {
//                        ProgressChanged?.Invoke(this, CurrentProgress);
//                        await Task.Delay(TimeSpan.FromSeconds(1));
//                        Debug.WriteLine($"Прогресс {CurrentProgress}");
//                        CurrentProgress += random.Next(1, 20);
//                    }
//                    IsCompleted = true;
//                    if (random.Next(2) == 0)
//                    {
//                        IsSuccess = true;
//                    }
//                    else
//                    {
//                        IsSuccess = false;
//                    }
//                    Debug.WriteLine(IsSuccess ? "Успешно" : "Ошибка");
//                    TaskCompleted?.Invoke(this, IsSuccess);
//                });
//            }catch (Exception ex)
//            {
//                Debug.WriteLine(ex);
//            }
//        }


//        public void StopTask()
//        {
//            if (isTaskRunning && task != null && !task.IsCompleted)
//            {
//                isTaskRunning = false;
//                task.Wait(); // Дождитесь завершения задачи
//            }
//        }

//        public void ContinueTask()
//        {
//            if (!isTaskRunning && task != null && task.IsCompleted)
//            {
//                isTaskRunning = true;
//                task = Task.Run(async () =>
//                {
//                    // Возобновите выполнение задачи с сохраненного состояния
//                    // В этом примере можно просто вызвать StartTask(), но это зависит от вашей логики.
//                    StartTask();
//                });
//            }
//        }
//    }
//}


using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AvaloniaEidos.Models
{
    public class TaskModel
    {
        private int newId = 0;
        private bool isTaskRunning = false;
        private Task task;

        public int Id { get; private set; }
        public string Type { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSuccess { get; set; }
        public int CurrentProgress { get; set; }

        private readonly Random random;

        public event EventHandler<double> ProgressChanged;
        public event EventHandler<bool> TaskCompleted;

        public TaskModel(string type)
        {
            this.Id = newId++;
            this.Type = type;
            this.IsCompleted = false;
            this.CurrentProgress = 0;
            this.random = new Random();
        }

        public void StartTask()
        {
            if (!isTaskRunning)
            {
                isTaskRunning = true;
                task = Task.Run(async () =>
                {
                    CurrentProgress += random.Next(1, 30);
                    while (CurrentProgress < 100)
                    {
                        ProgressChanged?.Invoke(this, CurrentProgress);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        Debug.WriteLine($"Прогресс {CurrentProgress}");
                        CurrentProgress += random.Next(1, 30);
                    }
                    IsCompleted = true;
                    if (random.Next(2) == 0)
                    {
                        IsSuccess = true;
                    }
                    else
                    {
                        IsSuccess = false;
                    }
                    Debug.WriteLine(IsSuccess ? "Успешно" : "Ошибка");
                    TaskCompleted?.Invoke(this, IsSuccess);
                });
            }
        }

        public void StopTask()
        {
            if (isTaskRunning && task != null && !task.IsCompleted)
            {
                isTaskRunning = false;
                task.Wait();
            }
        }

        public void ContinueTask()
        {
            try
            {
                if (!isTaskRunning && task != null && task.IsCompleted)
                {
                    isTaskRunning = true;
                    task = task.ContinueWith(async (prevTask) =>
                    {
                        StartTask();
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}

