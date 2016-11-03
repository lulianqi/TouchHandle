using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchHandle.myTool
{
    class myDataStruct
    {

    }

    public class KeyMapValue
    {
        string _indexValue;
        string _key;
        string _value;
        public KeyMapValue(string indexValue, string key, string value)
        {
            _indexValue = indexValue;
            _key = key;
            _value = value;
        }

        public string IndexValue
        {
            get
            {
                return _indexValue;
            }
            set
            {
                _indexValue = value;
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

    }
}
