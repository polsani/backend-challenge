import random
from datetime import datetime, timedelta

num_records = 10_000_000
output_file = "sample.txt"

transaction_types = ['1', '2', '3', '4', '5', '8', '9']
store_owners = [
    'JOÃO MACEDO   ', 'MARIA JOSEFINA', 'MARCOS PEREIRA', 'JOSÉ COSTA    ',
    'PEDRO SANTOS  ', 'ANA SILVA     ', 'CARLOS LIMA   ', 'FERNANDA ROCHA'
]
store_names = [
    'BAR DO JOÃO        ', 'LOJA DO Ó - MATRIZ ', 'MERCADO DA AVENIDA ',
    'MERCEARIA 3 IRMÃOS ', 'LOJA DO Ó - FILIAL ', 'SUPERMERCADO CENTRA',
    'PADARIA BOM DIA    ', 'FARMÁCIA SAÚDE     '
]

start_date = datetime(2019, 1, 1)
end_date = datetime(2024, 12, 31)
date_range = (end_date - start_date).days

print(f"Gerando {num_records:,} registros...")

with open(output_file, 'w', encoding='utf-8') as f:
    for i in range(num_records):
        trans_type = random.choice(transaction_types)
        random_date = start_date + timedelta(days=random.randint(0, date_range))
        date_str = random_date.strftime('%Y%m%d')
        value_str = str(random.randint(100, 9999999999)).zfill(10)
        cpf = ''.join([str(random.randint(0, 9)) for _ in range(11)])
        card = ''.join([str(random.randint(0, 9)) for _ in range(4)]) + '****' + ''.join([str(random.randint(0, 9)) for _ in range(4)])
        time_str = f"{random.randint(0, 23):02d}{random.randint(0, 59):02d}{random.randint(0, 59):02d}"
        owner = random.choice(store_owners)
        store = random.choice(store_names)
        
        line = f"{trans_type}{date_str}{value_str}{cpf}{card}{time_str}{owner}{store}\n"
        f.write(line)
        
        if (i + 1) % 10_000_000 == 0:
            print(f"Progresso: {i + 1:,} registros ({((i + 1) / num_records * 100):.1f}%)")

print(f"\nConcluído! Arquivo: {output_file}")