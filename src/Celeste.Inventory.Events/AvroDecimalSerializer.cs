namespace Celeste.Inventory.Events;

using System.Numerics;
using Avro;

/// <summary>
///     Serializes domain decimals to Avro decimal logical values.
/// </summary>
public static class AvroDecimalSerializer
{
    private const int DefaultScale = 2;

    /// <summary>
    ///     Converts a domain decimal to an Avro decimal logical value.
    /// </summary>
    /// <param name="value">
    ///     The domain decimal value.
    /// </param>
    /// <param name="scale">
    ///     The Avro decimal scale.
    /// </param>
    /// <returns>
    ///     The Avro decimal value.
    /// </returns>
    public static AvroDecimal Serialize(
        decimal value,
        int scale = DefaultScale)
    {
        var normalized = decimal.Round(value, scale, MidpointRounding.AwayFromZero);
        var unscaled = normalized * Pow10(scale);

        return new AvroDecimal(new BigInteger(unscaled), scale);
    }

    /// <summary>
    ///     Converts an Avro decimal logical value to a domain decimal.
    /// </summary>
    /// <param name="value">
    ///     The Avro decimal value.
    /// </param>
    /// <returns>
    ///     The domain decimal value.
    /// </returns>
    public static decimal Deserialize(AvroDecimal value)
    {
        return (decimal)value;
    }

    /// <summary>
    ///     Converts an Avro decimal logical value to its Avro bytes representation.
    /// </summary>
    /// <param name="value">
    ///     The Avro decimal value.
    /// </param>
    /// <returns>
    ///     The Avro bytes representation.
    /// </returns>
    public static byte[] ToBytes(AvroDecimal value)
    {
        return value.UnscaledValue.ToByteArray(isUnsigned: false, isBigEndian: true);
    }

    /// <summary>
    ///     Converts Avro decimal bytes to an Avro decimal logical value.
    /// </summary>
    /// <param name="value">
    ///     The Avro bytes representation.
    /// </param>
    /// <param name="scale">
    ///     The Avro decimal scale.
    /// </param>
    /// <returns>
    ///     The Avro decimal value.
    /// </returns>
    public static AvroDecimal FromBytes(
        byte[] value,
        int scale = DefaultScale)
    {
        return new AvroDecimal(new BigInteger(value, isUnsigned: false, isBigEndian: true), scale);
    }

    private static decimal Pow10(int scale)
    {
        var result = 1m;
        for (var i = 0; i < scale; i++)
            result *= 10m;

        return result;
    }
}
