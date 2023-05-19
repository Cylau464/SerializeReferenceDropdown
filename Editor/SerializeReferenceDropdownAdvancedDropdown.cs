using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace SerializeReferenceDropdown.Editor
{
    public class SerializeReferenceDropdownAdvancedDropdown : AdvancedDropdown
    {
        private readonly IEnumerable<string> typeNames;
        //private readonly Dictionary<string, List<string>> typeNamesWithPath;
        private readonly Dictionary<string, Dictionary<int, string>> typeNamesWithPath;
        private readonly Dictionary<AdvancedDropdownItem, int> itemAndIndexes =
            new Dictionary<AdvancedDropdownItem, int>();

        private readonly Action<int> onSelectedTypeIndex;

        //public SerializeReferenceDropdownAdvancedDropdown(AdvancedDropdownState state, IEnumerable<string> typeNames,
        //    Action<int> onSelectedNewType) :
        //    base(state)
        //{
        //    this.typeNames = typeNames;
        //    onSelectedTypeIndex = onSelectedNewType;
        //}

        private struct Item
        {
            public string Name;
            public int Index;

            public Item(string name, int index)
            {
                Name = name;
                Index = index;
            }
        }

        public SerializeReferenceDropdownAdvancedDropdown(AdvancedDropdownState state, IEnumerable<KeyValuePair<string, string>> typeNamesWithPath,
            Action<int> onSelectedNewType) :
            base(state)
        {
            this.typeNamesWithPath = ConvertToDictionary(typeNamesWithPath);
            onSelectedTypeIndex = onSelectedNewType;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Types");
            itemAndIndexes.Clear();

            //var index = 0;

            //if (typeNames != null)
            //{
            //    foreach (var typeName in typeNames)
            //        AddItem(root, typeName, ref index);
            //}

            foreach (KeyValuePair<string, Dictionary<int, string>> kvp in typeNamesWithPath)
            {
                string[] path = kvp.Key.Split("/");
                AdvancedDropdownItem currentSection = root;

                foreach (string sectionName in path)
                    currentSection = AddSection(currentSection, sectionName);

                foreach (KeyValuePair<int, string> typeNameAndIndex in kvp.Value)
                    AddItem(currentSection, typeNameAndIndex);
            }

            return root;
        }

        private AdvancedDropdownItem AddSection(AdvancedDropdownItem root, string sectionName)
        {
            if (sectionName.Equals(string.Empty))
                return root;

            AdvancedDropdownItem section = root.children.FirstOrDefault(x => x.name.Equals(sectionName));

            if (section != null)
            {
                root = section;
            }
            else
            {
                section = new AdvancedDropdownItem(sectionName);
                root.AddChild(section);
                root = section;
            }

            return root;
        }

        private void AddItem(AdvancedDropdownItem section, KeyValuePair<int, string> typeNameAndIndex/*, ref int index*/)
        {
            AdvancedDropdownItem item = new AdvancedDropdownItem(typeNameAndIndex.Value);
            section.AddChild(item);
            itemAndIndexes.Add(item, typeNameAndIndex.Key);
            //index++;
        }

        private Dictionary<string, Dictionary<int, string>> ConvertToDictionary(IEnumerable<KeyValuePair<string, string>> typeNamesWithPath)
        {
            Dictionary<string, Dictionary<int, string>> result = new Dictionary<string, Dictionary<int, string>>();
            int index = 0;

            foreach (KeyValuePair<string, string> kvp in typeNamesWithPath)
            {
                if (result.ContainsKey(kvp.Key) == false)
                    result.Add(kvp.Key, new Dictionary<int, string>());

                result[kvp.Key].Add(index, kvp.Value);
                index++;
            }

            return result;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            if (itemAndIndexes.TryGetValue(item, out var index))
            {
                onSelectedTypeIndex.Invoke(index);
            }
        }
    }
}