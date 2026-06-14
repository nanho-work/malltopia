using System;
using System.Collections.Generic;

namespace Malltopia
{
    public class CustomerOrderService
    {
        public CustomerOrder CreateOrder(
            CustomerOrderConfigData config,
            CustomerTypeData customerType,
            ProductOrderCandidate[] candidates,
            bool isFirstOrder,
            Random random)
        {
            random = random ?? new Random();
            var eligibleProducts = GetEligibleProducts(config, candidates, isFirstOrder);

            if (eligibleProducts.Count == 0)
            {
                return new CustomerOrder
                {
                    customerTypeId = customerType != null ? customerType.customerTypeId : string.Empty,
                    lines = new CustomerOrderLine[0]
                };
            }

            var maxTypes = config != null ? Math.Max(1, config.maxDistinctProductTypes) : 1;
            var distinctTypeCountMax = Math.Min(maxTypes, eligibleProducts.Count);
            var distinctTypeCount = random.Next(1, distinctTypeCountMax + 1);
            var selectedProductIds = PickDistinctUniform(eligibleProducts, distinctTypeCount, random);
            var lines = new CustomerOrderLine[selectedProductIds.Count];

            for (var i = 0; i < selectedProductIds.Count; i++)
            {
                lines[i] = new CustomerOrderLine
                {
                    productId = selectedProductIds[i],
                    quantity = RollQuantity(config, customerType, random)
                };
            }

            return new CustomerOrder
            {
                customerTypeId = customerType != null ? customerType.customerTypeId : string.Empty,
                lines = lines
            };
        }

        public ProductUnitTask[] CreateProductUnitTasks(CustomerOrder order)
        {
            if (order == null || order.lines == null)
            {
                return new ProductUnitTask[0];
            }

            var tasks = new List<ProductUnitTask>();

            for (var lineIndex = 0; lineIndex < order.lines.Length; lineIndex++)
            {
                var line = order.lines[lineIndex];

                if (line == null)
                {
                    continue;
                }

                var quantity = Math.Max(0, line.quantity);

                for (var unitIndex = 0; unitIndex < quantity; unitIndex++)
                {
                    tasks.Add(new ProductUnitTask
                    {
                        productId = line.productId,
                        lineIndex = lineIndex,
                        unitIndex = unitIndex
                    });
                }
            }

            return tasks.ToArray();
        }

        private static List<string> GetEligibleProducts(
            CustomerOrderConfigData config,
            ProductOrderCandidate[] candidates,
            bool isFirstOrder)
        {
            var results = new List<string>();

            if (candidates == null)
            {
                return results;
            }

            for (var i = 0; i < candidates.Length; i++)
            {
                var candidate = candidates[i];

                if (candidate == null || string.IsNullOrEmpty(candidate.productId))
                {
                    continue;
                }

                if (isFirstOrder && config != null && config.firstOrderMode == FirstOrderMode.StartingStandOnly)
                {
                    if (candidate.isStartingStand)
                    {
                        results.Add(candidate.productId);
                        return results;
                    }

                    continue;
                }

                if (candidate.isUnlocked && candidate.hasActiveProductionSlot)
                {
                    results.Add(candidate.productId);
                }
            }

            return results;
        }

        private static List<string> PickDistinctUniform(List<string> source, int count, Random random)
        {
            var pool = new List<string>(source);
            var selected = new List<string>();

            while (selected.Count < count && pool.Count > 0)
            {
                var index = random.Next(0, pool.Count);
                selected.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return selected;
        }

        private static int RollQuantity(CustomerOrderConfigData config, CustomerTypeData customerType, Random random)
        {
            var min = config != null ? Math.Max(1, config.minQuantityPerProduct) : 1;
            var max = config != null ? Math.Max(min, config.maxQuantityPerProduct) : 1;
            var quantity = random.Next(min, max + 1);

            if (customerType != null && customerType.orderQuantityMultiplier > 0f && Math.Abs(customerType.orderQuantityMultiplier - 1f) > 0.0001f)
            {
                quantity = Math.Max(1, (int)Math.Round(quantity * customerType.orderQuantityMultiplier));
            }

            return quantity;
        }
    }
}
