using System.Collections.Concurrent;
using System.IO;

namespace HtmlPerformanceKit;

/// <summary>
/// Thread-safe pool of HtmlReader.
/// </summary>
public class HtmlReaderPool
{
    private readonly HtmlReaderOptions? options;
    private readonly ConcurrentStack<HtmlReader> pool = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlReaderPool" /> class.
    /// </summary>
    public HtmlReaderPool()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlReaderPool" /> class.
    /// </summary>
    /// <param name="options">Options to configure <see cref="HtmlReader" /> behaviour.</param>
    public HtmlReaderPool(HtmlReaderOptions? options)
    {
        this.options = options;
    }

    /// <summary>
    /// Rents an instance of <see cref="HtmlReader" /> from the pool.
    /// </summary>
    /// <param name="textReader">TextReader instance to read from.</param>
    /// <returns>Pooled <see cref="HtmlReader" /> instance.</returns>
    public HtmlReader Rent(TextReader textReader)
    {
        if (pool.TryPop(out var htmlReader))
        {
            htmlReader.Initialize(textReader);
            return htmlReader;
        }

        return new HtmlReader(textReader, options);
    }

    /// <summary>
    /// Rents an instance of <see cref="HtmlReader" /> from the pool.
    /// </summary>
    /// <param name="stream">Stream instance to read from.</param>
    /// <returns>Pooled <see cref="HtmlReader" /> instance.</returns>
    public HtmlReader Rent(Stream stream)
    {
        if (pool.TryPop(out var htmlReader))
        {
            htmlReader.Initialize(new StreamReader(stream));
            return htmlReader;
        }

        return new HtmlReader(stream, options);
    }

    /// <summary>
    /// Returns an instance of <see cref="HtmlReader" /> to the pool.
    /// </summary>
    /// <param name="htmlReader"><see cref="HtmlReader" /> instance.</param>
    public void Return(HtmlReader htmlReader)
    {
        htmlReader.Reset();
        pool.Push(htmlReader);
    }
}