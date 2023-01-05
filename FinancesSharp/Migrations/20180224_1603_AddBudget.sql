CREATE TABLE `Budgets` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `MiscellaneousBudget` DECIMAL(18,2) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`)
);

CREATE TABLE `BudgetItems` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` longtext,
  `BudgetedAmount` decimal(18,2) NOT NULL,
  `Budget_Id` int(11) DEFAULT NULL,

  PRIMARY KEY (`Id`),
  KEY `IX_Budget_Id` (`Budget_Id`) USING BTREE,
  
  CONSTRAINT `FK_BudgetItems_Budgets_Budget_Id` 
    FOREIGN KEY (`Budget_Id`) REFERENCES `Budgets` (`Id`)
);

CREATE TABLE `CategoryBudgetItems` (
  `Category_Id` int(11) NOT NULL,
  `BudgetItem_Id` int(11) NOT NULL,

  PRIMARY KEY (`Category_Id`,`BudgetItem_Id`),
  KEY `IX_Category_Id` (`Category_Id`) USING BTREE,
  KEY `IX_BudgetItem_Id` (`BudgetItem_Id`) USING BTREE,

  CONSTRAINT `FK_CategoryBudgetItems_BudgetItems_BudgetItem_Id` 
    FOREIGN KEY (`BudgetItem_Id`) REFERENCES `BudgetItems` (`Id`) 
    ON DELETE CASCADE ON UPDATE CASCADE,

  CONSTRAINT `FK_CategoryBudgetItems_Categories_Category_Id` 
    FOREIGN KEY (`Category_Id`) REFERENCES `Categories` (`Id`) 
    ON DELETE CASCADE ON UPDATE CASCADE
)