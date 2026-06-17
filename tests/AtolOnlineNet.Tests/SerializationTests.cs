using System.Text.Json;
using AtolOnlineNet;
using AtolOnlineNet.Models;
using AtolOnlineNet.V1_05;
using AtolOnlineNet.Serialization;

namespace AtolOnlineNet.Tests;

public class SerializationTests
{
    [Fact]
    public void Receipt_SerializesWithSnakeCaseAndWireEnumValues()
    {
        var request = new ReceiptRegistrationRequest
        {
            ExternalId = "ext-1",
            Timestamp = new DateTime(2017, 2, 1, 13, 45, 0),
            Receipt = new Receipt
            {
                Client = new Client { Email = "kkt@kkt.ru" },
                Company = new Company
                {
                    Email = "chek@romashka.ru",
                    Sno = TaxationSystem.Osn,
                    Inn = "1234567891",
                    PaymentAddress = "http://magazin.ru/",
                },
                Items =
                [
                    new Item
                    {
                        Name = "колбаса",
                        Price = 1000.00m,
                        Quantity = 0.3m,
                        Sum = 300.00m,
                        MeasurementUnit = "кг",
                        PaymentMethod = PaymentMethod.FullPayment,
                        PaymentObject = PaymentObject.Commodity,
                        Vat = new Vat { Type = VatType.Vat20 },
                    },
                ],
                Payments = [new Payment { Type = PaymentType.Cashless, Sum = 400.0m }],
                Vats = [new Vat { Type = VatType.Vat20, Sum = 50.0m }],
                Total = 400.0m,
            },
            Service = new Service { CallbackUrl = "http://testtest" },
        };

        var json = JsonSerializer.Serialize(request, AtolJson.Options);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("ext-1", root.GetProperty("external_id").GetString());
        Assert.Equal("01.02.2017 13:45:00", root.GetProperty("timestamp").GetString());

        var receipt = root.GetProperty("receipt");
        Assert.Equal("osn", receipt.GetProperty("company").GetProperty("sno").GetString());
        Assert.Equal("http://magazin.ru/", receipt.GetProperty("company").GetProperty("payment_address").GetString());

        var item = receipt.GetProperty("items")[0];
        Assert.Equal("full_payment", item.GetProperty("payment_method").GetString());
        Assert.Equal("commodity", item.GetProperty("payment_object").GetString());
        Assert.Equal("vat20", item.GetProperty("vat").GetProperty("type").GetString());
        Assert.Equal("кг", item.GetProperty("measurement_unit").GetString());

        Assert.Equal(1, receipt.GetProperty("payments")[0].GetProperty("type").GetInt32());
        Assert.Equal("http://testtest", root.GetProperty("service").GetProperty("callback_url").GetString());
    }

    [Fact]
    public void NullProperties_AreOmitted()
    {
        var client = new Client { Email = "a@b.ru" };
        var json = JsonSerializer.Serialize(client, AtolJson.Options);

        Assert.Contains("\"email\"", json);
        Assert.DoesNotContain("phone", json);
        Assert.DoesNotContain("name", json);
        Assert.DoesNotContain("inn", json);
    }

    [Theory]
    [InlineData(PaymentObject.NonOperatingGain, "non-operating_gain")]
    [InlineData(PaymentObject.Commodity, "commodity")]
    [InlineData(PaymentObject.ResortFee, "resort_fee")]
    public void PaymentObject_UsesExactWireValues(PaymentObject value, string expected)
    {
        var json = JsonSerializer.Serialize(value, AtolJson.Options);
        Assert.Equal($"\"{expected}\"", json);
    }

    [Fact]
    public void CorrectionInfo_BaseDate_UsesDateOnlyFormat()
    {
        var info = new CorrectionInfo
        {
            Type = CorrectionType.Self,
            BaseDate = new DateTime(2017, 7, 25),
            BaseNumber = "1175",
        };

        var json = JsonSerializer.Serialize(info, AtolJson.Options);
        using var doc = JsonDocument.Parse(json);
        Assert.Equal("self", doc.RootElement.GetProperty("type").GetString());
        Assert.Equal("25.07.2017", doc.RootElement.GetProperty("base_date").GetString());
    }

    [Fact]
    public void UnknownEnumValue_ThrowsWithIssueLink()
    {
        var ex = Assert.Throws<AtolOnlineUnknownEnumValueException>(
            () => JsonSerializer.Deserialize<Vat>("""{"type":"vat999"}""", AtolJson.Options));

        Assert.Equal(typeof(VatType), ex.EnumType);
        Assert.Equal("vat999", ex.WireValue);
        Assert.Contains(AtolOnlineUnknownEnumValueException.IssuesUrl, ex.Message);
    }

    [Fact]
    public void AtolErrorType_Unknown_IsAValidWireValueNotAnError()
    {
        // "unknown" is a documented error.type value and must map, not throw.
        var error = JsonSerializer.Deserialize<AtolError>(
            """{"error_id":"x","code":0,"text":"oops","type":"unknown"}""", AtolJson.Options)!;
        Assert.Equal(AtolErrorType.Unknown, error.Type);
    }

    [Fact]
    public void ReportPayload_DeserializesLargeFiscalAttribute()
    {
        const string json = """
        {
          "uuid": "2ea26f17-0884-4f08-b120-306fc096a58f",
          "status": "done",
          "timestamp": "12.04.2017 20:15:08",
          "payload": {
            "total": 1598,
            "fns_site": "www.nalog.ru",
            "fn_number": "1110000100238211",
            "shift_number": 23,
            "receipt_datetime": "12.04.2017 20:16:00",
            "fiscal_receipt_number": 6,
            "fiscal_document_number": 133,
            "ecr_registration_number": "0000111118041361",
            "fiscal_document_attribute": 3449555941
          }
        }
        """;

        var report = JsonSerializer.Deserialize<DocumentReportResponse>(json, AtolJson.Options)!;

        Assert.Equal(DocumentStatus.Done, report.Status);
        Assert.Equal(3449555941L, report.Payload!.FiscalDocumentAttribute);
        Assert.Equal(1598m, report.Payload.Total);
        Assert.Equal(new DateTime(2017, 4, 12, 20, 16, 0), report.Payload.ReceiptDatetime);
    }
}
