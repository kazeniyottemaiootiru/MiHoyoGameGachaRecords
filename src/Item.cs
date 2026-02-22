using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace MiHoyoGameGachaRecords
{
    /// <summary>
    /// 设置数据
    /// </summary>
    public class SettingsData
    {
        public int language { get; set; }
        public int theme { get; set; }
        public string? GenshinPath { get; set; }
        public string? HoukaiStarRailPath { get;set; }
        public string? ZZZPath { get; set; }
    }

    /// <summary>
    /// web内容
    /// </summary>
    public class Item
    {
        public int retcode { get; set; }
        public string? message { get; set; }
        public Data? data { get; set; }
    }

    public class Data
    {
        public string? page { get; set; }
        public string? size { get; set; }
        public string? total { get; set; }
        public List<GItem>? list { get; set; }
        public string? region { get; set; }
    }

    public class GItem
    {
        public string? rank_type { get; set; }
        public string? name { get; set; }
        public string? time { get; set; }
        public string? uid { get; set; }
        public string? gacha_type { get; set; }
        public string? item_id { get; set; }
        public string? count { get; set; }
        public string? lang { get; set; }
        public string? item_type { get; set; }
        public string? id { get; set; }
    }

    /// <summary>
    /// 出金记录
    /// </summary>
    public class Mem
    {
        public string? name { get; set; }
        public int count { get; set; }
        public string time { get; set; } = string.Empty;
    }

    /// <summary>
    /// 常驻角色
    /// </summary>
    public class PermanentCharacters
    {
        public string? name { get; set; }
        public string time { get; set; } = string.Empty;
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class PermanentCharacter : INotifyPropertyChanged
    {
        private string _character = "";
        public string Character
        {
            get => _character;
            set { if (_character != value) { _character = value; OnPropertyChanged(nameof(Character)); } }
        }

        private string _year = DateTime.Now.Year.ToString();
        public string Year
        {
            get => _year;
            set { if (_year != value) { _year = value; OnPropertyChanged(nameof(Year)); } }
        }

        private string _month = DateTime.Now.Month.ToString("00");
        public string Month
        {
            get => _month;
            set { if (_month != value) { _month = value; OnPropertyChanged(nameof(Month)); } }
        }

        private string _day = DateTime.Now.Day.ToString("00");
        public string Day
        {
            get => _day;
            set { if (_day != value) { _day = value; OnPropertyChanged(nameof(Day)); } }
        }

        // 用于删除当前行的命令，绑定到DataTemplate中的Button
        private ICommand? _deleteCommand;
        public ICommand? DeleteCommand
        {
            get => _deleteCommand;
            set { if (_deleteCommand != value) { _deleteCommand = value; OnPropertyChanged(nameof(DeleteCommand)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// 构建图形界面
    /// </summary>
    public class MemViewModel
    {
        public string Name { get; }
        public int Value { get; }

        public double BarWidth { get; }
        public Brush BarBrush { get; }

        public string ExistMark { get; }

        public MemViewModel(
            Mem mem,
            int max,
            int warn,
            bool exists,
            string existText)
        {
            Name = mem.name;
            Value = mem.count;

            double percent = Math.Min(mem.count, max) / (double)max;
            BarWidth = percent * 600;

            BarBrush = mem.count > warn
                ? new SolidColorBrush(Colors.Red)
                : new SolidColorBrush(Colors.Green);

            ExistMark = exists ? existText : string.Empty;
        }
    }

}