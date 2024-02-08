using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaEidos.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace AvaloniaEidos.ViewModels;

public partial class MainViewModel : ObservableObject
{
    #region Properties
    [ObservableProperty]
    private TaskModel _task;

    [ObservableProperty]
    private ObservableCollection<TaskModel> _tasks;


    [ObservableProperty]
    private ObservableCollection<ProgressBar> _progressBars;

    [ObservableProperty]
    private string _logText;
    #endregion

    #region Commands
    public ICommand AddTaskCommand { get; }
    public ICommand StopAllTasksCommand { get; }
    public ICommand ContinueAllTasksCommand { get; }

    #endregion

    public MainViewModel() 
    {
        AddTaskCommand = new RelayCommand(AddTask);
        StopAllTasksCommand = new RelayCommand(StopAllTasks);
        ContinueAllTasksCommand = new RelayCommand(ContinueAllTasks);

        Tasks = new ObservableCollection<TaskModel>();
        ProgressBars = new ObservableCollection<ProgressBar>();

    }

    private void AddTask(object parameter)
    {
        // создаем экземпляр задачи
        Task = new TaskModel(parameter as string);
        LogText += $"{DateTime.UtcNow} Была добавлена задача {Task.Type} c ID={Task.Id} \n";
        Debug.WriteLine($"Создана задача типа {Task.Type} с ID={Task.Id}");
        // добавляем в общий список задач
        Tasks.Add(Task);
        // создаем progress bar для этой задачи
        ProgressBar currentProgressBar = new ProgressBar();
        currentProgressBar.Tag = Task.Type;
        ProgressBars.Add(currentProgressBar);
        Task.StartTask();
        Task.ProgressChanged += (sender, progress) =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                currentProgressBar.Value = progress;
                Debug.WriteLine("Прогресс бару присвоено значение " + progress.ToString());
            });
        };
        Task.TaskCompleted += (sender, isSuccessful) =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                if (isSuccessful)
                {
                    LogText += $"{DateTime.UtcNow} Задача №{Task.Id} ({Task.Type}) завершена успешно.\n";
                }
                else
                {
                    LogText += $"{DateTime.UtcNow} Задача №{Task.Id} ({Task.Type}) завершена с ошибкой.\n";
                }
            });
        };
    }
    private void StopAllTasks(object parameter)
    {
        foreach (var task in Tasks)
        {
            task.StopTask();
        }
    }

    private void ContinueAllTasks(object parameter)
    {
        foreach (var task in Tasks)
        {
            task.ContinueTask();
        }
    }
}
