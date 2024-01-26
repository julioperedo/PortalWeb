using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities
{
    public enum StatusType : Int32
    {
        NoAction = 0,
        Insert = 1,
        Update = 2,
        Delete = 3
    }

    namespace Filters
    {
        public enum Operators : Int32
        {
            Equal = 0,
            Different = 1,
            Likes = 2,
            NotLikes = 3,
            HigherThan = 4,
            HigherOrEqualThan = 5,
            LowerThan = 6,
            LowerOrEqualThan = 7,
            //Between = 8,
            In = 9,
            NotIn = 10,
            IsNull = 11,
            IsNotNull = 12
        }
        public enum LogicalOperators : Int32
        {
            None = 0,
            And = 1,
            Or = 2
        }
        public class Field
        {
            public string Name;
            public object Value;
            public Operators Operator;
            public LogicalOperators LogicalOperator;
            public Field() { }
            public Field(string Name, object Value, Operators Operator = Operators.Equal)
            {
                this.Name = Name;
                this.Value = Value;
                this.Operator = Operator;
            }
            public Field(LogicalOperators LogicalOperator)
            {
                this.LogicalOperator = LogicalOperator;
            }

            public static Field New(string Name, object Value, Operators Operator = Operators.Equal)
            {
                return new Field(Name, Value, Operator);
            }

            public static Field LogicalAnd()
            {
                return new Field(LogicalOperators.And);
            }

            public static Field LogicalOr()
            {
                return new Field(LogicalOperators.Or);
            }

        }


    }

    namespace Settings
    {
        public class SAPSettings
        {
            public string Server { get; set; }
            public int Port { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
            public string DBSA { get; set; }
            public string DBIQ { get; set; }
            public string DBLA { get; set; }
        }
    }
}
