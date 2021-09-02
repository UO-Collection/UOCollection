//===================================================================
//===================================================================
// Copyright (C) 2021  3HMonkey @UO-Collection
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// NOTICE: Some script parts come from other collections. If a creator
// is known, it is listed here: Wyatt of RUOSI, andreakarasho 
//===================================================================
//===================================================================

using System.Collections.Generic;
using System.IO;

namespace UOCollection.FileHandling
{
    public class MULFileIndex : BaseFileIndex
    {
        private readonly string _indexPath;

        public MULFileIndex(string idxFile, string mulFile)
            : base(mulFile)
        {
            _indexPath = idxFile;
        }

        public override bool FilesExist
        {
            get { return File.Exists(_indexPath) && base.FilesExist; }
        }

        protected override FileIndexEntry[] ReadEntries()
        {
            var entries = new List<FileIndexEntry>();

            var length = (int)((new FileInfo(_indexPath).Length / 3) / 4);

            using (var index = new FileStream(_indexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bin = new BinaryReader(index);

                var count = (int)(index.Length / 12);

                for (var i = 0; i < count && i < length; ++i)
                {
                    var entry = new FileIndexEntry
                    {
                        Lookup = bin.ReadInt32(),
                        Length = bin.ReadInt32(),
                        Extra = bin.ReadInt32()
                    };

                    entries.Add(entry);
                }

                for (var i = count; i < length; ++i)
                {
                    var entry = new FileIndexEntry
                    {
                        Lookup = -1,
                        Length = -1,
                        Extra = -1
                    };

                    entries.Add(entry);
                }
            }

            return entries.ToArray();
        }
    }
}
