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
        public DateTime currentWeekday { get; set; }
        public DateTime currentDate { get; set; }
        public string Grade { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClassOne { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClassTwo { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClassThree { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClassFour { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClassFive { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClassSix { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClassSeven { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClassEight { get; set; }
    }
}
