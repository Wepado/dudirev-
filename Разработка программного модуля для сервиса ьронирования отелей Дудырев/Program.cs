bool isUpdateStatus = false;
string message = "";

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Order> repo = new()
{
    new Order(1, 901437475 , 14, "������ ������� �. ", "��� ������", "������� 11", "���", DateTime.Parse("2024-10-10T00:00:00"), DateTime.Parse("2024-10-05T00:00:00")),
    new Order(2, 901251555, 18, "������� �������� �.", "����� �������", "�������� 52", "���", DateTime.Parse("2024-10-12T00:00:00"), DateTime.Parse("2024-10-06T00:00:00"))
};

app.MapGet("/orders", () =>
{
    var result = new
    {
        Orders = repo,
        Notification = isUpdateStatus ? message : null
    };

    isUpdateStatus = false; 
    message = ""; 

    return Results.Json(result);
});

app.MapGet("/orders/{number}", (int number) => repo.Find(o => o.Number == number));

app.MapPost("/orders", (Order newOrder) =>
{
    repo.Add(newOrder);
    return Results.Created($"/orders/{newOrder.Number}", newOrder);
});

app.MapPut("/orders/{number}", (int number, OrderUpdateDTO updateDto) =>
{
    var order = repo.FirstOrDefault(o => o.Number == number);
    if (order == null)
    {
        return Results.NotFound();
    }

    
    if (updateDto.CheckInDate1 != order.CheckInDate1)
        order.CheckInDate1 = updateDto.CheckInDate1;

    if (updateDto.CheckOutDate1 != order.CheckOutDate1)
    {
        message += $"�����������: ���� ������ �������� ��� ������ {order.Number} �� {updateDto.CheckOutDate1}\n";
        isUpdateStatus = true;
        order.CheckOutDate1 = updateDto.CheckOutDate1;
    }

    
    if (order.CheckOutDate1 < DateTime.Now)
    {
        message += $"�����������: ���� �� ������ {order.Number} ��������! ���� ���������: {order.CheckOutDate1}\n";
        isUpdateStatus = true;
    }

    order.Shel = updateDto.shel; // � shel ������������ ��� ���������, � ���-�� ���  ������������� ��������, ������, ������� ��������
    order.Master = updateDto.Master;

    return Results.Ok(order);
});

app.Run();

record class OrderUpdateDTO(DateTime CheckOutDate1, DateTime CheckInDate1, string shel, string Master);
class Order
{
    int number;
    int tele;
    int apart;
    string fio;
    string shel;
    string adres;
    string master;
    DateTime checkOutDate;
    DateTime checkInDate;

    public Order(int number, int tele, int apart, string fio, string shel, string adres, string master, DateTime checkOutDate1, DateTime checkInDate1)
    {
        Number = number;
        Tele = tele;
        Apart = apart;
        Fio = fio;
        Shel = shel;
        Adres = adres;
        Master = master;
        CheckOutDate1 = checkOutDate1;
        CheckInDate1 = checkInDate1;
    }

    public int Number { get => number; set => number = value; }
    public int Tele { get => tele; set => tele = value; }
    public int Apart { get => apart; set => apart = value; }
    public string Fio { get => fio; set => fio = value; }
    public string Shel { get => shel; set => shel = value; }
    public string Adres { get => adres; set => adres = value; }
    public DateTime CheckOutDate1 { get => checkOutDate; set => checkOutDate = value; }
    public DateTime CheckInDate1 { get => checkInDate; set => checkInDate = value; }
    public string Master { get => master; set => master = value; }
}