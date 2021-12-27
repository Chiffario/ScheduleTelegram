using System;
using System.Collections.Generic;

using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

public class Formats
{
    // JSON для изначального форматирования гугл-таблицы
    public class Lessons
    {
        public string MajorDimension { get; set; }
        public string Range { get; set; }
        public List<List<string>> Values { get; set; }
        public object ETag { get; set; }
    }
    public class LessonsReformatted
    {
        public string ClassOne { get; set; }
        public string ClassTwo { get; set; }
        public string ClassThree { get; set; }
        public string ClassFour { get; set; }
        public string ClassFive { get; set; }
        public string ClassSix { get; set; }
        public string ClassSeven { get; set; }
        public string ClassEight { get; set; }
    }
    public enum SubjNames
    {

    }
}
