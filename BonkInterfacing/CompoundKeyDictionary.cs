using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace BonkInterfacing
{
    [Serializable]
    public class CompoundKeyDictionary<TPrimary, TSecondary, TValue> :
        ICollection<CompoundKeyValueSet<TPrimary, TSecondary, TValue>>
    {
        private List<CompoundKeyValueSet<TPrimary, TSecondary, TValue>> dict { get; set; }
        private Dictionary<TSecondary, TValue> getDict { get; set; }
        private Type ThisType { get; }
        public int Count { get => dict.Count; }
        public bool IsReadOnly { get; private set; }

        

        public CompoundKeyDictionary()
        {
            ThisType = typeof(CompoundKeyDictionary<TPrimary, TSecondary, TValue>);
            getDict = new Dictionary<TSecondary, TValue>();
            dict = new List<CompoundKeyValueSet<TPrimary, TSecondary, TValue>>();
        }

        public async Task ImportFromJsonAsync(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string s = await reader.ReadToEndAsync();
            reader.Close();
            CompoundKeyDictionary<TPrimary, TSecondary, TValue> import =
                JsonSerializer.Deserialize(s, ThisType) as CompoundKeyDictionary<TPrimary, TSecondary, TValue>;
            dict = import.dict;
        }

        public async Task ExportToJsonAsync(string filePath)
        {
            string s = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
            StreamWriter writer = new StreamWriter(filePath);
            await writer.WriteAsync(s);
            writer.Close();
        }

        //WARNING: This dictionary is indipendant of the CompoundKeyDictionary and modifying it WILL NOT modify the contents of the
        // dictionary it came from.
        //TODO: fix it so that it is not the case
        public Dictionary<TSecondary, TValue> this[TPrimary primaryKey]
        {            
            get
            {
                UpdateGetDictoinary(primaryKey);
                return getDict;
            }
        }

        //TODO: this is a slow ass method, 6ms for a collection with one element, uses a lot of JIT. make better somehow
        private void UpdateGetDictoinary(TPrimary primaryKey)
        {
            List<CompoundKeyValueSet<TPrimary, TSecondary, TValue>> og = dict.FindAll(s => s.PrimaryKey.Equals(primaryKey));    //1ms
            if (og.Count == 0)
            {
                throw new KeyNotFoundException();
            }
            else
            {
                Dictionary<TSecondary, TValue> returnDict = new Dictionary<TSecondary, TValue>();   //1ms
                foreach (CompoundKeyValueSet<TPrimary, TSecondary, TValue> set in og)       //2ms
                {
                    returnDict.Add(set.SecondaryKey, set.Value);                                    //2ms(total)
                }
                getDict = returnDict;
            }
            
        }

        public TValue this[TPrimary primaryKey, TSecondary secondaryKey]
        {
            get
            {
                CompoundKeyValueSet<TPrimary, TSecondary, TValue> og = dict.Find
                    (s => (s.PrimaryKey.Equals(primaryKey) && s.SecondaryKey.Equals(secondaryKey)));
                if (og.Value is null)
                {
                    throw new KeyNotFoundException();
                }
                else
                {
                    return og.Value;
                }

            }
            set
            {
                CompoundKeyValueSet<TPrimary, TSecondary, TValue> og = dict.Find
                    (s => (s.PrimaryKey.Equals(primaryKey) && s.SecondaryKey.Equals(secondaryKey)));
                og.Value = value;
            }
        }

        public void Clear() => dict.Clear();
        public bool Contains(CompoundKeyValueSet<TPrimary, TSecondary, TValue> keyValueSet) => dict.Contains(keyValueSet);
        public void CopyTo(CompoundKeyValueSet<TPrimary, TSecondary, TValue>[] keyValueSetArr, int index)
            => dict.CopyTo(keyValueSetArr, index);
        public bool Remove(CompoundKeyValueSet<TPrimary, TSecondary, TValue> keyValueSet) => dict.Remove(keyValueSet);
        IEnumerator<CompoundKeyValueSet<TPrimary, TSecondary, TValue>>
            IEnumerable<CompoundKeyValueSet<TPrimary, TSecondary, TValue>>.GetEnumerator() => dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();

        //Checks to see if a compound key pair exists
        public bool KeyPairExists(TPrimary primaryKey, TSecondary secondaryKey)
        {
            return dict.Exists(p => (p.PrimaryKey.Equals(primaryKey) && p.SecondaryKey.Equals(secondaryKey)));
        }

        public void Add(CompoundKeyValueSet<TPrimary, TSecondary, TValue> keyValueSet)
        {
            if (!KeyPairExists(keyValueSet.PrimaryKey, keyValueSet.SecondaryKey))
            {
                dict.Add(keyValueSet);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void Add(TPrimary primaryKey, TSecondary sexondaryKey, TValue val)
        {
            if (!KeyPairExists(primaryKey, sexondaryKey))
            {
                dict.Add(new CompoundKeyValueSet<TPrimary, TSecondary, TValue>(primaryKey,sexondaryKey,val));
            }
            else
            {
                throw new ArgumentException();
            }
        }

    }
}
