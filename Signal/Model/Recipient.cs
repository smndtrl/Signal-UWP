

using SQLite.Net.Attributes;
/** 
* Copyright (C) 2015 smndtrl
* 
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.util;

namespace Signal.Model
{
    public class Recipient
    {

        //private final Set<RecipientModifiedListener> listeners = Collections.newSetFromMap(new WeakHashMap<RecipientModifiedListener, Boolean>());
        [PrimaryKey, AutoIncrement]
        public long RecipientId { get; private set; }

        public string Number { get; private set; }
        public string Name { get; private set; }

        //private Drawable contactPhoto;
        private Uri contactUri;

        /*Recipient(String number, Drawable contactPhoto,
                  long recipientId, ListenableFutureTask<RecipientDetails> future)
        {
            this.number = number;
            this.contactPhoto = contactPhoto;
            this.recipientId = recipientId;

            future.addListener(new FutureTaskListener<RecipientDetails>() {
      @Override
      public void onSuccess(RecipientDetails result)
        {
            if (result != null)
            {
                Set<RecipientModifiedListener> localListeners;

                synchronized(Recipient.this) {
                    Recipient.this.name = result.name;
                    Recipient.this.number = result.number;
                    Recipient.this.contactUri = result.contactUri;
                    Recipient.this.contactPhoto = result.avatar;

                    localListeners = new HashSet<>(listeners);
                    listeners.clear();
                }

                for (RecipientModifiedListener listener : localListeners)
                    listener.onModified(Recipient.this);
            }
        }

        @Override
      public void onFailure(Throwable error)
        {
            Log.w("Recipient", error);
        }
    });
  }*/

        public Recipient(String name, String number, long recipientId/*, Uri contactUri, Drawable contactPhoto*/)
        {
            Number = number;
            RecipientId = recipientId;
            //this.contactUri = contactUri;
            Name = name;
            //this.contactPhoto = contactPhoto;
        }

        /*public  Uri getContactUri()
        {
            return this.contactUri;
        }*/

        public String getName()
        {
            return Name;
        }

        public String getNumber()
        {
            return Number;
        }

        public long getRecipientId()
        {
            return RecipientId;
        }

        public bool isGroupRecipient()
        {
            return GroupUtil.isEncodedGroup(Number);
        }

        /*public synchronized void addListener(RecipientModifiedListener listener)
        {
            listeners.add(listener);
        }

        public synchronized void removeListener(RecipientModifiedListener listener)
        {
            listeners.remove(listener);
        }Ü
        */
        public string ShortString
        {
            get
            {
                return (Name == null ? Number : Name);
            }
        }

        /*
        public synchronized Drawable getContactPhoto()
        {
            return contactPhoto;
        }*/

        public static Recipient getUnknownRecipient()
        {
            return new Recipient("Unknown", "Unknown", -1/*, null,
                                 ContactPhotoFactory.getDefaultContactPhoto(context, null)*/);
        }

        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (o == null || !(o is Recipient)) return false;

            Recipient that = (Recipient)o;

            return RecipientId == that.RecipientId;
        }

        public override int GetHashCode()
        {
            return 31 + (int)this.RecipientId;
        }

        /*public interface RecipientModifiedListener
        {
            public void onModified(Recipient recipient);
        }*/
    }
}
