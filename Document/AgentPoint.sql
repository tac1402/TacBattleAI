SELECT ap.Id, [Discriminator], [Info], [CapitalId], [LastOrderNeedMoney]
	  ,rawP.ProductName, rawP.[count], rawP.CostPrice, rawP.EnterpriseId
	  ,p.ProductName, p.[count], p.CostPrice, p.EnterpriseId

FROM AgentPoint as ap
left join LKeyValue_String_Product as rawStorage on rawStorage.LDictionaryId = RawStorageId
left join LKeyValue_String_Product as storage on storage.LDictionaryId = StorageId
left join Product as rawP on rawP.Id = rawStorage.ValueId
left join Product as p on p.Id = storage.ValueId

where Discriminator <> 'SmallHause'
order by ap.Id

