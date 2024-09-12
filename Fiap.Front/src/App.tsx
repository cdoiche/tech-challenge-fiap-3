import { useEffect, useState } from 'react'
import { getContacts, deleteContact, createContact, updateContact } from './services/Contacts';
import { ContactTable } from './components/Table';
import Contact from './models/Contact';
import './App.css'

function App() {
  const getContactFromSelection = (): Contact | undefined => {
    if (selectedRows && selectedRows.length === 1) {
      return (contacts || []).filter((contact: Contact) => contact.id === selectedRows[0])[0];
    }
  };

  const [contacts, setContacts] = useState<Contact[]>();

  const [selectedRows, setSelectedRows] = useState<number[]>([]);

  const [searchByDDD, setSearchByDDD] = useState<string>('');

  const [contactName, setContactName] = useState<string>('');
  const [contactDDD, setContactDDD] = useState<string>('');
  const [contactPhone, setContactPhone] = useState<string>('');
  const [contactEmail, setContactEmail] = useState<string>('');

  const [contactUpdatedName, setContactUpdatedName] = useState<string>('');
  const [contactUpdatedDDD, setContactUpdatedDDD] = useState<string>('');
  const [contactUpdatedPhone, setContactUpdatedPhone] = useState<string>('');
  const [contactUpdatedEmail, setContactUpdatedEmail] = useState<string>('');

  const [contactCreationError, setContactCreationError] = useState<boolean>(false);
  const [contactUpdateError, setContactUpdateError] = useState<boolean>(false);

  useEffect(() => {
  console.log('selectedRows', selectedRows);
  }, [selectedRows]);


  const handleSearch = async (ev: any) => {
    console.log('ev.target.value', ev.target.value);
    setSearchByDDD(ev.target.value);
  };

  const openUpdateModal = () => {
    setContactUpdatedName(getContactFromSelection()?.nome ?? '');
    setContactUpdatedDDD(getContactFromSelection()?.ddd ?? '');
    setContactUpdatedPhone(getContactFromSelection()?.telefone ?? '');
    setContactUpdatedEmail(getContactFromSelection()?.email ?? '');

    document.getElementById('updateModal').showModal();
  }

  const handleCreate = async () => {
    if (!contactName || !contactDDD || !contactPhone || !contactEmail) {
      alert('Please fill in all contact information.');
      return;
    }
  
    const newContact: Contact = {
      nome: contactName,
      ddd: contactDDD,
      telefone: contactPhone,
      email: contactEmail,
    };
  
    try {
      await createContact(newContact);
      
      await fetchContacts({ ddd: '' });
      
      setContactName('');
      setContactDDD('');
      setContactPhone('');
      setContactEmail('');

      setContactCreationError(false);

      document.getElementById('createModal').close();
    } catch (error) {
      setContactCreationError(true);
    }
  };

  const handleUpdate = async () => {
    if (!contactUpdatedName || !contactUpdatedDDD || !contactUpdatedPhone || !contactUpdatedEmail) {
      alert('Please fill in all contact information.');
      return;
    }
  
    const updatedContact: Contact = {
      id: getContactFromSelection()?.id,
      nome: contactUpdatedName,
      ddd: contactUpdatedDDD,
      telefone: contactUpdatedPhone,
      email: contactUpdatedEmail,
    };
  
    try {
      await updateContact(updatedContact);
      
      await fetchContacts({ ddd: '' });

      setContactUpdateError(false);

      document.getElementById('updateModal').close();
    } catch (error) {
      setContactUpdateError(true);
    }
  };

  const handleDelete = (selectedRows: number[]): void => {
    const errors: number[] = [];

    const removals = selectedRows.map(async (rowId) => {
      try {
        return await deleteContact(rowId);
      } catch (error) {
        errors.push(rowId);
        console.log('error', rowId)
      }
    });

    Promise.all(([...removals])).then(async () => await fetchContacts({ ddd: '' }));

    if (errors.length > 0) {
      console.error("Teve erro ao apagar as linhas de id:", errors.join(', '));
    }
  }

  const fetchContacts = async ({ ddd }: { ddd?: string }) => {
    try {
      const allContacts = await getContacts(ddd);
      setContacts(allContacts);
    } catch (error) {
      console.error('Error:', error);
    }
  };

  useEffect(() => {
    fetchContacts({ddd: searchByDDD});
  }, [searchByDDD]);

  return (
    <>
      <div className="container mx-auto">
        <div className="w-full py-2 gap-2 flex justify-around">
          <input type="text" placeholder="Busca por DDD" value={searchByDDD} onChange={handleSearch} className="input input-bordered w-full max-w-xs" />
          <button className="btn btn-outline btn-success" onClick={() => document.getElementById('createModal').showModal()}>
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-person-plus" viewBox="0 0 16 16">
            <path d="M6 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6m2-3a2 2 0 1 1-4 0 2 2 0 0 1 4 0m4 8c0 1-1 1-1 1H1s-1 0-1-1 1-4 6-4 6 3 6 4m-1-.004c-.001-.246-.154-.986-.832-1.664C9.516 10.68 8.289 10 6 10s-3.516.68-4.168 1.332c-.678.678-.83 1.418-.832 1.664z"/>
            <path d="M13.5 5a.5.5 0 0 1 .5.5V7h1.5a.5.5 0 0 1 0 1H14v1.5a.5.5 0 0 1-1 0V8h-1.5a.5.5 0 0 1 0-1H13V5.5a.5.5 0 0 1 .5-.5"/>
          </svg>
              Inserir
          </button>
          <button className="btn btn-outline btn-secondary" disabled={selectedRows.length === 0} onClick={() => handleDelete(selectedRows)}>
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-trash" viewBox="0 0 16 16">
              <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0z"/>
              <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4zM2.5 3h11V2h-11z"/>
            </svg>
              Apagar
          </button>
          <button className="btn btn-outline btn-info" disabled={!selectedRows || selectedRows?.length !== 1} onClick={openUpdateModal}>
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-pencil" viewBox="0 0 16 16">
            <path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708l-10 10a.5.5 0 0 1-.168.11l-5 2a.5.5 0 0 1-.65-.65l2-5a.5.5 0 0 1 .11-.168zM11.207 2.5 13.5 4.793 14.793 3.5 12.5 1.207zm1.586 3L10.5 3.207 4 9.707V10h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.293zm-9.761 5.175-.106.106-1.528 3.821 3.821-1.528.106-.106A.5.5 0 0 1 5 12.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.468-.325"/>
          </svg>
              Atualizar
          </button>
        </div>
        <ContactTable contacts={contacts} onSelectRows={setSelectedRows} />
      </div>

      <dialog id="createModal" className="modal">
        <div className="modal-box">
          { contactCreationError && (
            <div role="alert" className="alert alert-error mt-4 mb-2">
              <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
              <span>Erro na criação do contato: verifique os dados inseridos e tente novamente!</span>
            </div>
          ) }
          <form method="dialog">
            <button className="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
          </form>
          <h3 className="font-bold text-lg pb-4">Inserir Contato</h3>
          <div className="flex flex-col">
            <div className="py-2"><input type="text" placeholder="Nome" value={contactName} onChange={(ev) => setContactName(ev.target.value)} className="input input-bordered w-full max-w-xs" /></div>
            <div className="py-2"><input type="text" placeholder="DDD" value={contactDDD} onChange={(ev) => setContactDDD(ev.target.value)} className="input input-bordered w-full max-w-xs" /></div>
            <div className="py-2"><input type="text" placeholder="Telefone" value={contactPhone} onChange={(ev) => setContactPhone(ev.target.value)} className="input input-bordered w-full max-w-xs" /></div>
            <div className="py-2"><input type="text" placeholder="Email" value={contactEmail} onChange={(ev) => setContactEmail(ev.target.value)} className="input input-bordered w-full max-w-xs" /></div>
            <div className="py-2 self-end"><button className="btn btn-outline btn-success" onClick={handleCreate}>Salvar</button></div>
          </div>
        </div>
      </dialog>

      <dialog id="updateModal" className="modal">
        <div className="modal-box">
        { contactUpdateError && (
            <div role="alert" className="alert alert-error mt-4 mb-2">
              <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
              <span>Erro na atualização do contato: verifique os dados inseridos e tente novamente!</span>
            </div>
          ) }
          <form method="dialog">
            <button className="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
          </form>
          <h3 className="font-bold text-lg pb-4">Atualizar Contato</h3>
          <div className="flex flex-col">
            <div className="py-2"><input type="text" placeholder="Nome" value={contactUpdatedName} onChange={(ev) => setContactUpdatedName(ev.target.value)} className="input input-bordered w-full max-w-xs" /></div>
            <div className="py-2"><input type="text" placeholder="DDD" value={contactUpdatedDDD} onChange={(ev) => setContactUpdatedDDD(ev.target.value)} className="input input-bordered w-full max-w-xs" /></div>
            <div className="py-2"><input type="text" placeholder="Telefone" value={contactUpdatedPhone} onChange={(ev) => setContactUpdatedPhone(ev.target.value)} className="input input-bordered w-full max-w-xs" /></div>
            <div className="py-2"><input type="text" placeholder="Email" value={contactUpdatedEmail} onChange={(ev) => setContactUpdatedEmail(ev.target.value)} className="input input-bordered w-full max-w-xs" /></div>
            <div className="py-2 self-end"><button className="btn btn-outline btn-success" onClick={handleUpdate}>Salvar</button></div>
          </div>
        </div>
      </dialog>
    </>
  )
}

export default App
