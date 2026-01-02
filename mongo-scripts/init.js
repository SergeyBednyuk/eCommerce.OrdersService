// Switch to your database
var db = db.getSiblingDB("OrdersDatabase");

// 1. Create User (Keep this)
db.createUser({
    user: "root",
    pwd: "example",
    roles: [{ role: "readWrite", db: "OrdersDatabase" }]
});

var orders = [
  {
    _id: "4d9b6010-48e6-4a0e-bd4e-461c85c32c1d",
    OrderId: "4d9b6010-48e6-4a0e-bd4e-461c85c32c1d",
    UserId: "c32f8b42-60e6-4c02-90a7-9143ab37189f",
    OrderDate: ISODate("2050-10-20T08:00:00Z"),
    Total: NumberDecimal("2799.98"),
    OrderItems: [
      {
        ProductId: "1a9df78b-3f46-4c3d-9f2a-1b9f69292a77",
        ItemPrice: NumberDecimal("1299.99"),
        Quantity: 1,
        TotalPrice: NumberDecimal("1299.99"),
      },
      {
        ProductId: "2c8e8e7c-97a3-4b11-9a1b-4dbe681cfe17",
        ItemPrice: NumberDecimal("1499.99"),
        Quantity: 1,
        TotalPrice: NumberDecimal("1499.99"),
      },
    ],
  },
  {
    _id: "62c2fb9c-b36e-497e-b0b7-f07c6c3c22b2",
    OrderId: "62c2fb9c-b36e-497e-b0b7-f07c6c3c22b2",
    UserId: "8ff22c7d-18c7-4ef0-a0ac-988ecb2ac7f5",
    OrderDate: ISODate("2050-10-21T09:00:00Z"),
    Total: NumberDecimal("969.95"),
    OrderItems: [
      {
        ProductId: "3f3e8b3a-4a50-4cd0-8d8e-1e178ae2cfc1",
        ItemPrice: NumberDecimal("249.99"),
        Quantity: 1,
        TotalPrice: NumberDecimal("249.99"),
      },
      {
        ProductId: "4c9b6f71-6c5d-485f-8db2-58011a236b63",
        ItemPrice: NumberDecimal("179.99"),
        Quantity: 4,
        TotalPrice: NumberDecimal("719.96"),
      },
    ],
  },
  {
    _id: "e3f6d6b7-bc84-48e3-8d22-961e1e084f0e",
    OrderId: "e3f6d6b7-bc84-48e3-8d22-961e1e084f0e",
    UserId: "8ff22c7d-18c7-4ef0-a0ac-988ecb2ac7f5",
    OrderDate: ISODate("2050-10-22T10:00:00Z"),
    Total: NumberDecimal("5499.88"),
    OrderItems: [
      {
        ProductId: "3f3e8b3a-4a50-4cd0-8d8e-1e178ae2cfc1",
        ItemPrice: NumberDecimal("249.99"),
        Quantity: 10,
        TotalPrice: NumberDecimal("2499.90"),
      },
      {
        ProductId: "2c8e8e7c-97a3-4b11-9a1b-4dbe681cfe17",
        ItemPrice: NumberDecimal("1499.99"),
        Quantity: 2,
        TotalPrice: NumberDecimal("2999.98"),
      },
    ],
  },
  {
    _id: "af168b29-b6c5-45ed-a4f1-19c04f368d1a",
    OrderId: "af168b29-b6c5-45ed-a4f1-19c04f368d1a",
    UserId: "8ff22c7d-18c7-4ef0-a0ac-988ecb2ac7f5",
    OrderDate: ISODate("2050-10-23T11:00:00Z"),
    Total: NumberDecimal("5039.94"),
    OrderItems: [
      {
        ProductId: "4c9b6f71-6c5d-485f-8db2-58011a236b63",
        ItemPrice: NumberDecimal("179.99"),
        Quantity: 3,
        TotalPrice: NumberDecimal("539.97"),
      },
      {
        ProductId: "2c8e8e7c-97a3-4b11-9a1b-4dbe681cfe17",
        ItemPrice: NumberDecimal("1499.99"),
        Quantity: 3,
        TotalPrice: NumberDecimal("4499.97"),
      },
    ],
  },
  {
    _id: "3a0e5c1a-446e-4e0c-90dc-b87e0576cf36",
    OrderId: "3a0e5c1a-446e-4e0c-90dc-b87e0576cf36",
    UserId: "8ff22c7d-18c7-4ef0-a0ac-988ecb2ac7f5",
    OrderDate: ISODate("2050-10-24T12:00:00Z"),
    Total: NumberDecimal("6679.94"),
    OrderItems: [
      {
        ProductId: "1a9df78b-3f46-4c3d-9f2a-1b9f69292a77",
        ItemPrice: NumberDecimal("1299.99"),
        Quantity: 5,
        TotalPrice: NumberDecimal("6499.95"),
      },
      {
        ProductId: "4c9b6f71-6c5d-485f-8db2-58011a236b63",
        ItemPrice: NumberDecimal("179.99"),
        Quantity: 1,
        TotalPrice: NumberDecimal("179.99"),
      },
    ],
  },
  {
    _id: "a07e908e-57b1-49f0-b18e-4860b0948cf2",
    OrderId: "a07e908e-57b1-49f0-b18e-4860b0948cf2",
    UserId: "8ff22c7d-18c7-4ef0-a0ac-988ecb2ac7f5",
    OrderDate: ISODate("2050-10-25T13:00:00Z"),
    Total: NumberDecimal("859.96"),
    OrderItems: [
      {
        ProductId: "3f3e8b3a-4a50-4cd0-8d8e-1e178ae2cfc1",
        ItemPrice: NumberDecimal("249.99"),
        Quantity: 2,
        TotalPrice: NumberDecimal("499.98"),
      },
      {
        ProductId: "4c9b6f71-6c5d-485f-8db2-58011a236b63",
        ItemPrice: NumberDecimal("179.99"),
        Quantity: 2,
        TotalPrice: NumberDecimal("359.98"),
      },
    ],
  },
  {
    _id: "b8f3fbfc-5648-40f8-bb51-f158b32f77d9",
    OrderId: "b8f3fbfc-5648-40f8-bb51-f158b32f77d9",
    UserId: "c32f8b42-60e6-4c02-90a7-9143ab37189f",
    OrderDate: ISODate("2050-10-26T14:00:00Z"),
    Total: NumberDecimal("2849.97"),
    OrderItems: [
      {
        ProductId: "1a9df78b-3f46-4c3d-9f2a-1b9f69292a77",
        ItemPrice: NumberDecimal("1299.99"),
        Quantity: 2,
        TotalPrice: NumberDecimal("2599.98"),
      },
      {
        ProductId: "3f3e8b3a-4a50-4cd0-8d8e-1e178ae2cfc1",
        ItemPrice: NumberDecimal("249.99"),
        Quantity: 1,
        TotalPrice: NumberDecimal("249.99"),
      },
    ],
  },
  {
    _id: "c6a05e2e-81c0-4a43-80d1-8fd6936318e2",
    OrderId: "c6a05e2e-81c0-4a43-80d1-8fd6936318e2",
    UserId: "c32f8b42-60e6-4c02-90a7-9143ab37189f",
    OrderDate: ISODate("2050-10-27T15:00:00Z"),
    Total: NumberDecimal("9499.94"),
    OrderItems: [
      {
        ProductId: "5d7e36bf-65c3-4a71-bf97-740d561d8b65",
        ItemPrice: NumberDecimal("1999.99"),
        Quantity: 1,
        TotalPrice: NumberDecimal("1999.99"),
      },
      {
        ProductId: "2c8e8e7c-97a3-4b11-9a1b-4dbe681cfe17",
        ItemPrice: NumberDecimal("1499.99"),
        Quantity: 5,
        TotalPrice: NumberDecimal("7499.95"),
      },
    ],
  },
  {
    _id: "d66c9b87-0f4b-482d-b87b-fc5b96b59871",
    OrderId: "d66c9b87-0f4b-482d-b87b-fc5b96b59871",
    UserId: "c32f8b42-60e6-4c02-90a7-9143ab37189f",
    OrderDate: ISODate("2050-10-28T16:00:00Z"),
    Total: NumberDecimal("2999.98"),
    OrderItems: [
      {
        ProductId: "2c8e8e7c-97a3-4b11-9a1b-4dbe681cfe17",
        ItemPrice: NumberDecimal("1499.99"),
        Quantity: 1,
        TotalPrice: NumberDecimal("1499.99"),
      },
      {
        ProductId: "5d7e36bf-65c3-4a71-bf97-740d561d8b65",
        ItemPrice: NumberDecimal("1499.99"),
        Quantity: 1,
        TotalPrice: NumberDecimal("1499.99"),
      },
    ],
  },
  {
    _id: "e2a3ff6b-ba0e-463e-bc07-aa4421317a53",
    OrderId: "e2a3ff6b-ba0e-463e-bc07-aa4421317a53",
    UserId: "c32f8b42-60e6-4c02-90a7-9143ab37189f",
    OrderDate: ISODate("2024-03-01T17:00:00Z"),
    Total: NumberDecimal("9679.26"),
    OrderItems: [
      {
        ProductId: "5d7e36bf-65c3-4a71-bf97-740d561d8b65",
        ItemPrice: NumberDecimal("1999.99"),
        Quantity: 3,
        TotalPrice: NumberDecimal("5999.97"),
      },
      {
        ProductId: "6a14f510-72c1-42c8-9a5a-8ef8f3f45a0d",
        ItemPrice: NumberDecimal("49.99"),
        Quantity: 7,
        TotalPrice: NumberDecimal("3499.30"),
      },
      {
        ProductId: "4c9b6f71-6c5d-485f-8db2-58011a236b63",
        ItemPrice: NumberDecimal("179.99"),
        Quantity: 1,
        TotalPrice: NumberDecimal("179.99"),
      },
    ],
  },
];

console.log("Seeding OrdersDatabase...");

// Insert datadoc
db.orders.insertMany(orders);

console.log("Seeding Completed.");
