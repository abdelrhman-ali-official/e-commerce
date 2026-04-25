-- Check existing governorates
SELECT * FROM GovernorateShippingPrices;

-- If no records exist, run this to seed all 27 Egyptian governorates:
-- DELETE FROM GovernorateShippingPrices; -- Optional: clear existing records

INSERT INTO GovernorateShippingPrices (GovernorateName, ShippingPrice, DeliveryDays, IsActive)
VALUES
('Cairo', 70, 7, 1),
('Alexandria', 70, 7, 1),
('Giza', 70, 7, 1),
('Qalyubia', 70, 7, 1),
('Port Said', 70, 7, 1),
('Suez', 70, 7, 1),
('Luxor', 70, 7, 1),
('Aswan', 70, 7, 1),
('Asyut', 70, 7, 1),
('Beheira', 70, 7, 1),
('Beni Suef', 70, 7, 1),
('Dakahlia', 70, 7, 1),
('Damietta', 70, 7, 1),
('Faiyum', 70, 7, 1),
('Gharbia', 70, 7, 1),
('Ismailia', 70, 7, 1),
('Kafr El Sheikh', 70, 7, 1),
('Matruh', 70, 7, 1),
('Minya', 70, 7, 1),
('Monufia', 70, 7, 1),
('New Valley', 70, 7, 1),
('North Sinai', 70, 7, 1),
('Qena', 70, 7, 1),
('Red Sea', 70, 7, 1),
('Sharqia', 70, 7, 1),
('Sohag', 70, 7, 1),
('South Sinai', 70, 7, 1);

-- Verify the insert
SELECT COUNT(*) AS TotalGovernorates FROM GovernorateShippingPrices;
