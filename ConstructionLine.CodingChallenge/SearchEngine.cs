using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    /// <summary>
    /// Contains the item filtering logic.
    /// </summary>
    public class SearchEngine
    {
        private readonly Dictionary<Guid, IEnumerable<Guid>> _colourDictionary;
        private readonly Dictionary<Guid, IEnumerable<Guid>> _sizeDictionary;
        private readonly Dictionary<Guid, Shirt> _allShirts;
        private static readonly Dictionary<Guid, Color> _allColours = Color.All.ToDictionary(a => a.Id);
        private static readonly Dictionary<Guid, Size> _allSizes = Size.All.ToDictionary(a => a.Id);

        /// <summary>
        /// Constructs an instance of <see cref="SearchEngine"/>.
        /// </summary>
        /// <param name="shirts">The list of all shirts.</param>
        public SearchEngine(List<Shirt> shirts)
        {
            if(shirts == null)
            {
                throw new ArgumentNullException(nameof(shirts));
            }

            _colourDictionary = shirts.GroupBy(a => a.Color).ToDictionary(a => a.Key.Id, a => a.Select(b => b.Id));
            _sizeDictionary = shirts.GroupBy(a => a.Size).ToDictionary(a => a.Key.Id, a => a.Select(b => b.Id));
            _allShirts = shirts.ToDictionary(a => a.Id);
        }

        /// <summary>
        /// Performs a search on the list of all shirts.
        /// </summary>
        /// <param name="options">The search options.</param>
        /// <returns>The filtered list of shirts and property counts.</returns>
        public SearchResults Search(SearchOptions options)
        {
            if (options?.Sizes == null || options?.Colors == null)
            {
                throw new ArgumentNullException(nameof(options), "The field options and its properties cannot be null");
            }

            var colourIds = this.GetColourIds(options.Colors);
            var sizeIds = this.GetSizeIds(options.Sizes);
            var intersect = this.GetIntersect(colourIds, sizeIds, options.Colors.Any(), options.Sizes.Any());
            var colourCounts = Color.All.ToDictionary(a => a.Id, a => 0);
            var sizeCounts = Size.All.ToDictionary(a => a.Id, a => 0);
            var shirts = new List<Shirt>();

            foreach(var id in intersect)
            {
                var shirt = _allShirts[id];
                shirts.Add(shirt);
                colourCounts[shirt.Color.Id]++;
                sizeCounts[shirt.Size.Id]++;
            }

            return new SearchResults
            {
                Shirts = shirts,
                ColorCounts = colourCounts.Select(a => 
                new ColorCount
                { 
                    Color=_allColours[a.Key], 
                    Count =  a.Value,
                }).ToList(),
                SizeCounts = sizeCounts.Select(a => 
                new SizeCount
                { 
                    Size = _allSizes[a.Key], 
                    Count = a.Value,
                }).ToList(),
            };
        }

        private List<Guid> GetColourIds(List<Color> colours)
        {
            var colourIds = new List<Guid>();
            
            foreach (var colour in colours)
            {
                if (_colourDictionary.ContainsKey(colour.Id))
                {
                    colourIds.AddRange(_colourDictionary[colour.Id]);
                }
            }

            return colourIds;
        }

        private List<Guid> GetSizeIds(List<Size> sizes)
        {
            var sizeIds = new List<Guid>();

            foreach (var size in sizes)
            {
                if (_sizeDictionary.ContainsKey(size.Id))
                {
                    sizeIds.AddRange(_sizeDictionary[size.Id]);
                }
            }

            return sizeIds;
        }

        private IEnumerable<Guid> GetIntersect(
            List<Guid> colourIds, 
            List<Guid> sizeIds, 
            bool colourCriteria, 
            bool sizeCriteria)
        {
            IEnumerable<Guid> intersect;

            if (colourCriteria && sizeCriteria)
            {
                intersect = colourIds.Intersect(sizeIds);
            }

            else if (colourCriteria)
            {
                intersect = colourIds;
            }

            else if (sizeCriteria)
            {
                intersect = sizeIds;
            }

            else
            {
                intersect = _allShirts.Keys;
            }

            return intersect;
        }
    }
}