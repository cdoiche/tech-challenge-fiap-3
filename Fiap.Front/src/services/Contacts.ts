import Contact from "../models/Contact";

export const getContacts = async (ddd?: string): Promise<Contact[]> => {
  const baseUrl = 'http://localhost:8080/ConsultarContato';
  const url = ddd ? `${baseUrl}?ddd=${ddd}` : baseUrl;

  const response = await fetch(url, {
    method: 'GET',
    headers: {
      'Accept': '*/*',
    },
  });

  if (!response.ok) {
    throw new Error(`Error fetching contacts: ${response.statusText}`);
  }

  const data = await response.json();
  return data as Contact[];
};

export const deleteContact = async (id: number): Promise<void> => {
  const baseUrl = 'http://localhost:8080/ExcluirContato';
  const url = `${baseUrl}?id=${id}`;

  const response = await fetch(url, {
    method: 'DELETE',
    headers: {
      'Accept': '*/*',
    },
  });

  if (!response.ok) {
    throw new Error(`Error deleting contacts: ${response.statusText}`);
  }
};

export const createContact = async (contact: Contact): Promise<Contact> => {
  const url = 'http://localhost:8080/CriarContato';

  const response = await fetch(url, {
    method: 'POST',
    headers: {
      'Accept': '*/*',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(contact),
  });

  if (!response.ok) {
    throw new Error(`Error creating contacts: ${response.statusText}`);
  }

  const data = await response.json();
  return data as Contact;
};

export const updateContact = async (updatedContact: Contact): Promise<void> => {
  const url = 'http://localhost:8080/AtualizarContato';

  const response = await fetch(url, {
    method: 'PUT',
    headers: {
      'Accept': '*/*',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(updatedContact),
  });

  if (!response.ok) {
    throw new Error(`Error updating contact: ${response.statusText}`);
  }
};
