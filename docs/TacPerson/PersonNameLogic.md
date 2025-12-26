---
title: PersonNameLogic
---

Позволяет создавать мужские и женские имена для персонажей. 

## LoadName
_LoadName(System.Random argRnd, string argMenText, string argWomenText)_

Создаст списки мужских и женских имен, разделяя их на имя и фамилию. В _argMenText_ ожидается встретить мужские именна, разделенные на строки, а в каждой строке в порядке Имя_Фамилия через пробел. Для _argWomenText_ аналогично для женских имен.

## GetUniqueName()
_string GetUniqueName(GenderType argGender)_

Получить уникальное (еще не использованную) комбинацию имя_фамилия, указав для мужщины или для женщины (argGender).

## GetSurname()

Получить случайную фамилию

## GetFamilyName
_string GetFamilyName(GenderType argGender, string argSurname)_

Получить уникальное (еще не использованную) комбинацию имя_фамилия, указав для мужщины или для женщины (argGender) и фамилию этой семьи (argSurname).

