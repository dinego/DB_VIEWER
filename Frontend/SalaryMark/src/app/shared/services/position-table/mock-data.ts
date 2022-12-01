import { Table } from "@/shared/models/position-table";

const data: Table = {
  header: [
    {
      colPos: 0,
      colName: "Cargo SalaryMark",
      nickName: "Cargo SalaryMark",
      editable: true,
      isChecked: true,
      disabled: true,
      visible: true,
      columnId: 1,
    },
    {
      colPos: 1,
      colName: "Perfil",
      nickName: "Perfil",
      editable: true,
      isChecked: true,
      disabled: false,
      visible: true,
      columnId: 2,
    },
    {
      colPos: 2,
      colName: "Nivel",
      nickName: "Nivel",
      editable: false,
      isChecked: true,
      disabled: false,
      visible: true,
      columnId: 3,
    },
    {
      colPos: 3,
      colName: "Setor",
      nickName: "Setor",
      editable: false,
      isChecked: true,
      disabled: true,
      visible: true,
      columnId: 4,
    },
    {
      colPos: 4,
      colName: "Base Horária",
      nickName: "Base Horária",
      editable: false,
      isChecked: true,
      disabled: false,
      visible: true,
      columnId: 5,
    },
    {
      colPos: 5,
      colName: "GSM",
      nickName: "GSM",
      editable: false,
      isChecked: true,
      disabled: false,
      visible: true,
      columnId: 6,
    },
    {
      colPos: 6,
      colName: "Ajuste Técnico",
      nickName: "Ajuste Técnico",
      editable: false,
      isChecked: true,
      disabled: false,
      visible: true,
      columnId: 7,
    },
    {
      colPos: 7,
      colName: "60%",
      nickName: "60%",
      editable: false,
      isChecked: false,
      disabled: false,
      visible: true,
      columnId: 8,
    },
  ],
  body: [
    [
      {
        colPos: 0,
        value: "Advogado",
        type: "string",
        tooltip: [
          {
            position: "Engenheiro X",
            amount: 3,
          },
          {
            position: "Engenheiro Y",
            amount: 1,
          },
        ],
        occupantClt: true,
        occupantPJ: false,
      },
      {
        colPos: 1,
        value: "Administrativo",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 2,
        value: "Especialista",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 3,
        value: "Jurídico",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 4,
        value: "220",
        type: "number",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 5,
        value: "25",
        type: "number",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 6,
        value: true,
        type: "boolean",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 7,
        value: "4234",
        type: "float",
        occupantClt: false,
        occupantPJ: false,
      },
    ],
    [
      {
        colPos: 0,
        value: "Analista Controladoria I",
        type: "string",
        occupantClt: true,
        occupantPJ: true,
      },
      {
        colPos: 1,
        value: "Administrativo",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 2,
        value: "Analista",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 3,
        value: "Financeiro",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 4,
        value: "220",
        type: "number",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 5,
        value: "19",
        type: "number",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 6,
        value: false,
        type: "boolean",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 7,
        value: "2639",
        type: "float",
        occupantClt: false,
        occupantPJ: false,
      },
    ],
    [
      {
        colPos: 0,
        value: "Analista Controladoria II",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 1,
        value: "Administrativo",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 2,
        value: "Analista",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 3,
        value: "Financeiro",
        type: "string",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 4,
        value: "220",
        type: "number",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 5,
        value: "24",
        type: "number",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 6,
        value: false,
        type: "boolean",
        occupantClt: false,
        occupantPJ: false,
      },
      {
        colPos: 7,
        value: "3903",
        type: "float",
        occupantClt: false,
        occupantPJ: false,
      },
    ],
  ],
};
export default data;
