import { Table } from '@/shared/models/map-table';

const data: Table = {
    header: [
      {
        colPos: 0,
        colName: "GSM",
        nickName: "BATATA",
        editable: true,
        isChecked: true,
        visible: true
      },
      {
        colPos: 1,
        colName: "Apoio",
        editable: true,
        isChecked: true,
        visible: true
      },
      {
        colPos: 2,
        colName: "Cliente",
        editable: false,
        isChecked: true,
        visible: true
      },
      {
        colPos: 3,
        colName: "Financeiro",
        editable: false,
        isChecked: true,
        visible: true
      },
      {
        colPos: 4,
        colName: "Jurídico",
        editable: false,
        isChecked: true,
        visible: true
      },
      {
        colPos: 5,
        colName: "RH",
        editable: false,
        isChecked: true,
        visible: true
      },
      {
        colPos: 6,
        colName: "Serviços Gerais",
        editable: false,
        isChecked: true,
        visible: true
      }
    ],
    body: [
      [
        {
          colPos: 0,
          value: "11"
        },
        {
          colPos: 1
        },
        {
          colPos: 2
        },
        {
          colPos: 3
        },
        {
          colPos: 4
        },
        {
          colPos: 5
        },
        {
          colPos: 6
        }
      ],
      [
        {
          colPos: 0,
          value: "12",
          occupantTypeId: 1,
        },
        {
          colPos: 1
        },
        {
          colPos: 2,
          value: "Operador Call Center",
        },
        {
          colPos: 3
        },
        {
          colPos: 4
        },
        {
          colPos: 5
        },
        {
          colPos: 6
        }
      ],
      [
        {
          colPos: 0,
          value: "13"
        },
        {
          colPos: 1,
          value: "Recepcionista Telefonista"
        },
        {
          colPos: 2
        },
        {
          colPos: 3
        },
        {
          colPos: 4
        },
        {
          colPos: 5,
          value: "Copeiro Jardineiro"
        },
        {
          colPos: 6
        }
      ],
      [
        {
          colPos: 0,
          value: "14"
        },
        {
          colPos: 1
        },
        {
          colPos: 2
        },
        {
          colPos: 3
        },
        {
          colPos: 4
        },
        {
          colPos: 5
        },
        {
          colPos: 6
        }
      ],
      [
        {
          colPos: 0,
          value: "15"
        },
        {
          colPos: 1,
          value: "Assistente Administrativo"
        },
        {
          colPos: 2
        },
        {
          colPos: 3
        },
        {
          colPos: 4,
          value: "Assistente Jurídico"
        },
        {
          colPos: 5,
          value: "Assistente RH",
          "tooltip": {
            "title": "Assistente RH",
            "description":  "1 func"
          }
        },
        {
          colPos: 6,
          value: "Porteiro"
        }
      ]
    ]
}

export default data;
