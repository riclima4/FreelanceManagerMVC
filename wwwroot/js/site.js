// Função para alternar a visibilidade da senha
function togglePasswordVisibility() {
    // Selecionando o input de senha e o ícone usando os IDs corretos do Login.razor
    const passwordInput = document.getElementById('Input.Password');
    const passwordIcon = document.getElementById('passwordIcon');
    
    if (!passwordInput) {
        console.error('Elemento de input de senha não encontrado');
        return;
    }

    // Obter o input real dentro do componente Syncfusion
    const actualInput = passwordInput.querySelector('input');
    
    if (!actualInput) {
        console.error('Input dentro do componente Syncfusion não encontrado');
        return;
    }
    
    // Alternar entre tipo password e text
    if (actualInput.type === "password") {
        actualInput.type = "text";
        passwordIcon.className = "fas fa-eye-slash";
    } else {
        actualInput.type = "password";
        passwordIcon.className = "fas fa-eye";
    }
}

// Função para atualizar a contagem de itens em cada coluna do Kanban
function updateKanbanHeaderCounts() {
    // Seletor para as colunas do Kanban
    const columns = document.querySelectorAll('.e-kanban .e-content-cells');
    const headers = document.querySelectorAll('.e-kanban .e-header-cells .e-header-text');
    
    if (columns.length === headers.length) {
        for (let i = 0; i < columns.length; i++) {
            // Conta o número de cards em cada coluna
            const cardCount = columns[i].querySelectorAll('.e-card').length;
            
            // Adiciona o atributo data-count ao texto do cabeçalho
            if (headers[i]) {
                headers[i].setAttribute('data-count', cardCount);
            }
        }
    }
}

// Verifica se o DOM foi carregado
document.addEventListener('DOMContentLoaded', function() {
    // Observa mudanças no Kanban
    const observer = new MutationObserver(function(mutations) {
        updateKanbanHeaderCounts();
    });
    
    // Inicia a observação após um curto atraso para garantir que o Kanban esteja carregado
    setTimeout(() => {
        const kanban = document.querySelector('.e-kanban');
        if (kanban) {
            updateKanbanHeaderCounts();
            observer.observe(kanban, { childList: true, subtree: true });
        }
    }, 1000);
});
